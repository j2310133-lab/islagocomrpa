using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ISLAGO_V3.Models.ViewModels;
using ISLAGO_V3.Entidad.Models;
using ISLAGO_V3.Datos.DBContext;
using ISLAGO_V3.Negocio.Interfaces;

namespace ISLAGO_V3.Controllers
{
    public class ArticuloController : Controller
    {
        private readonly DBContextISLAGO _context;
        private readonly IWebHostEnvironment _env;
        private readonly IArticuloServices _arServ;
        private readonly IBase64IMGSercies _imgServ;

        public ArticuloController(
            DBContextISLAGO c,
            IWebHostEnvironment env,
            IArticuloServices arServ,
            IBase64IMGSercies imgServ)
        {
            _context = c;
            _env = env;
            _arServ = arServ;
            _imgServ = imgServ;
        }

        public IActionResult Index()
        {
            return View();
        }

        // =================
        // LISTAR ARTICULO
        // =================
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            try
            {
                var lista = await _context.Articulos
                    .Where(a => a.Activo == true)
                    .Select(a => new
                    {
                        a.Id,
                        a.Nombre,
                        a.Descripcion,
                        a.Precio,
                        a.Stock,
                        Umedidum = a.IdumedidaNavigation.Nombre,
                        a.Activo,

                        Imagen = _context.Articuloimagens
                            .Where(ai => ai.Idarticulo == a.Id)
                            .Join(_context.Imagens,
                                ai => ai.Idimagen,
                                im => im.Id,
                                (ai, im) => new { im.Nombre, im.Fechapublicada })
                            .OrderByDescending(x => x.Fechapublicada)
                            .Select(x => x.Nombre)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                var resultado = lista.Select(a => new
                {
                    a.Id,
                    a.Nombre,
                    DescripcionCorta = string.IsNullOrEmpty(a.Descripcion)
                        ? ""
                        : (a.Descripcion.Length > 60
                            ? a.Descripcion.Substring(0, 60) + "..."
                            : a.Descripcion),

                    DescripcionCompleta = a.Descripcion,
                    a.Precio,
                    a.Stock,
                    a.Activo,
                    a.Umedidum,

                    // traer imagen desde base de datos
                    Imagen = a.Imagen != null
                        ? $"/img/articulos/{a.Imagen}"
                        : null
                });

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ===========================================
        // EndPoints de categoria y unidad medida
        // ===========================================
        [HttpGet]
        public async Task<IActionResult> ObtenerCategorias()
        {
            var lista = await _context.Categoria
                .Select(c => new
                {
                    c.Id,
                    c.Nombre
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUnidadesMedida()
        {
            var lista = await _context.Umedida
                .Select(u => new
                {
                    u.Id,
                    u.Nombre
                })
                .ToListAsync();

            return Ok(lista);
        }

        // =================
        // CREAR ARTICULO
        // =================

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ArticuloVM avm)
        {
            if (avm == null) return BadRequest("Datos vacios. Porfavor rellene todos los datos");

            try
            {

                // Mapeo Manual
                Articulo nuevoArticulo = new Articulo()
                {
                    Nombre = avm.Nombre,
                    Descripcion = avm.Descripcion,
                    Precio = avm.Precio,
                    Stock = avm.Stock,
                    Idumedida = avm.Idumedida,
                    Activo = avm.Activo
                }; 

                _context.Articulos.Add(nuevoArticulo);
                await _context.SaveChangesAsync();

                // ==============================
                // LISTAR Y GAURDAR CATEGORIAS
                // ==============================
                if (avm.CategoriasId != null && avm.CategoriasId.Any())
                {
                    var relaciones = avm.CategoriasId.Select(CId => new Articulocategorium
                    {
                        Idarticulo = nuevoArticulo.Id,
                        Idcategoria = CId
                    });

                    await _context.Set<Articulocategorium>().AddRangeAsync(relaciones);
                }

                // ==============================
                // CREAR IMAGENES Y GUARDARLAS
                // ==============================
                if(avm.ImagenesBase64 != null && avm.ImagenesBase64.Any())
                {
                    string rb = Path.Combine(_env.WebRootPath, "img", "articulos");

                    if(!Directory.Exists(rb)) Directory.CreateDirectory(rb);

                    foreach(var base64 in avm.ImagenesBase64)
                    {
                        if (string.IsNullOrWhiteSpace(base64)) continue;

                        // limpiar base64
                        var clean = base64.Contains(",")
                            ? base64.Split(',')[1]
                            : base64;

                        byte[] bytes = Convert.FromBase64String(clean);

                        // Detectar extensión
                        string ext = ".jpg";
                        if (base64.Contains("image/png")) ext = ".png";
                        if (base64.Contains("image/jpeg")) ext = ".jpg";

                        string FName = $"{Guid.NewGuid()}{ext}";
                        string path = Path.Combine(rb, FName);

                        await System.IO.File.WriteAllBytesAsync(path, bytes);

                        // ================================
                        // GUARDAR EN LA TABLA IMAGEN
                        // ================================
                        var imagen = new Imagen
                        {
                            Nombre = FName,
                            Ruta = $"/img/articulos/{FName}",
                            Tipo = ext,
                            Tamaño = bytes.Length / 1024,
                            Fechapublicada = DateTime.Now,
                            Estado = true
                        };

                        _context.Imagens.Add(imagen);
                        await _context.SaveChangesAsync();

                        // =================================
                        // RELACION ARTICULO IMAGEN
                        // =================================
                        var rel = new Articuloimagen
                        {
                            Idarticulo = nuevoArticulo.Id,
                            Idimagen = imagen.Id
                        };

                        _context.Set<Articuloimagen>().Add(rel);
                    }
                }

                // ================================
                // GUARDAR TODO EN BASE DE DATOS
                // ================================
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Articulo creado correctamente",
                    id = nuevoArticulo.Id
                });

            }
            catch(Exception e)
            {
                throw new Exception($"Error al intentar crear el articulo, error lanzado en controlador: {e.Message}", e);
            }
        }

        // ========================
        // Obtener por ID
        // ========================
        [HttpGet]
        public async Task<IActionResult> ObtenerPorId(int id)
        {

            try
            {

                var articulo = await _context.Articulos
                    .Where(a => a.Id == id)
                    .Select(a => new
                    {
                        a.Id,
                        a.Nombre,
                        a.Descripcion,
                        a.Precio,
                        a.Stock,
                        a.Activo,
                        a.Idumedida,

                        // ================
                        // CATEGORIAS
                        //================
                        Categorias = _context.Articulocategoria
                            .Where(ac => ac.Idarticulo == id)
                            .Select(ac => new
                            {
                                ac.Idcategoria,
                                ac.IdcategoriaNavigation.Nombre
                            }).ToList(),

                        Imagen = _context.Articuloimagens
                            .Where(am => am.Idarticulo == id)
                            .Join(_context.Imagens,
                            ai => ai.Idimagen,
                            im => im.Id,
                            (ai, im) => new
                            {
                                im.Ruta,
                                im.Fechapublicada
                            })
                            .OrderByDescending(x => x.Fechapublicada).ToList()
                    }).FirstOrDefaultAsync();
                
                if(articulo == null)
                {
                    return NotFound(new
                    {
                        message = "Articulo no encontrado"
                    });
                }

                return Ok(articulo);

            }
            catch(Exception e)
            {
                return BadRequest(new
                {
                    message = e.Message
                });
            }

        }

        // =======================
        // ACTUALIZAR ARTICULO
        // =======================
        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] ArticuloVM avm)
        {
            try
            {
                var articulo = await _context.Articulos
                    .FirstOrDefaultAsync(a => a.Id == avm.Id);

                if (articulo == null)
                {
                    return NotFound(new
                    {
                        message = "Artículo no encontrado"
                    });
                }

                // ==========================
                // ACTUALIZAR DATOS
                // ==========================
                articulo.Nombre = avm.Nombre;
                articulo.Descripcion = avm.Descripcion;
                articulo.Precio = avm.Precio;
                articulo.Stock = avm.Stock;
                articulo.Idumedida = avm.Idumedida;
                articulo.Activo = avm.Activo;

                // ==========================
                // ACTUALIZAR CATEGORIAS
                // ==========================

                var categoriasActuales = _context.Articulocategoria
                    .Where(ac => ac.Idarticulo == articulo.Id);

                _context.Articulocategoria.RemoveRange(categoriasActuales);

                if (avm.CategoriasId != null && avm.CategoriasId.Any())
                {
                    var nuevasCategorias = avm.CategoriasId.Select(c => new Articulocategorium
                    {
                        Idarticulo = articulo.Id,
                        Idcategoria = c
                    });

                    await _context.Articulocategoria.AddRangeAsync(nuevasCategorias);
                }

                // ==========================
                // NUEVAS IMAGENES
                // ==========================

                if (avm.ImagenesBase64 != null && avm.ImagenesBase64.Any())
                {
                    string rb = Path.Combine(_env.WebRootPath, "img", "articulos");

                    if (!Directory.Exists(rb))
                        Directory.CreateDirectory(rb);

                    foreach (var base64 in avm.ImagenesBase64)
                    {
                        if (string.IsNullOrWhiteSpace(base64))
                            continue;

                        var clean = base64.Contains(",")
                            ? base64.Split(',')[1]
                            : base64;

                        byte[] bytes = Convert.FromBase64String(clean);

                        string ext = ".jpg";

                        if (base64.Contains("image/png"))
                            ext = ".png";

                        if (base64.Contains("image/jpeg"))
                            ext = ".jpg";

                        string fileName = $"{Guid.NewGuid()}{ext}";

                        string path = Path.Combine(rb, fileName);

                        await System.IO.File.WriteAllBytesAsync(path, bytes);

                        var imagen = new Imagen
                        {
                            Nombre = fileName,
                            Ruta = $"/img/articulos/{fileName}",
                            Tipo = ext,
                            Tamaño = bytes.Length / 1024,
                            Fechapublicada = DateTime.Now,
                            Estado = true
                        };

                        _context.Imagens.Add(imagen);

                        await _context.SaveChangesAsync();

                        var rel = new Articuloimagen
                        {
                            Idarticulo = articulo.Id,
                            Idimagen = imagen.Id
                        };

                        _context.Articuloimagens.Add(rel);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Artículo actualizado correctamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // =======================
        // CAMBIAR ESTADO
        // =======================
        [HttpPut]
        public async Task<IActionResult> CambiarEstado(int id, bool activo)
        {
            try
            {
                var result = await _arServ.CambiarEstado(id, activo);

                if (!result)
                    return BadRequest(new
                    {
                        message = "No se pudo cambiar el estado del artículo."
                    });

                return Ok(new
                {
                    message = activo
                        ? "Artículo activado correctamente."
                        : "Artículo inhabilitado correctamente."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

    }
}

