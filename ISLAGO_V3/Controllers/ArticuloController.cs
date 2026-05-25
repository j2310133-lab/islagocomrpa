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
                 .Select(a => new
                 {
                     a.Id,

                     a.Nombre,
                     a.Marca,
                     a.Descripcion,

                     a.Stock,

                     a.PrecioCompra,

                     // AGREGAR ESTOS
                     a.PrecioVentaMinorista,
                     a.PrecioVentaMayorista,

                     UnidadMedida = a.IdumedidaNavigation != null
                         ? a.IdumedidaNavigation.Nombre
                         : "Sin unidad",

                     Proveedor = a.IdproveedorNavigation != null
                         ? a.IdproveedorNavigation.IdpersonaNavigation.Nombres
                         : "Ferreteria Lago",

                     Activo = a.Activo ?? false,

                     Imagen = _context.Articuloimagens
                         .Where(ai => ai.Idarticulo == a.Id)
                         .Join(
                             _context.Imagens,
                             ai => ai.Idimagen,
                             im => im.Id,
                             (ai, im) => new
                             {
                                 im.Nombre,
                                 im.Fechapublicada
                             }
                         )
                         .OrderByDescending(x => x.Fechapublicada)
                         .Select(x => x.Nombre)
                         .FirstOrDefault()
                 })
                 .ToListAsync();

                var resultado = lista.Select(a => new
                {
                    a.Id,

                    NombreCompleto = string.IsNullOrWhiteSpace(a.Marca)
                    ? a.Nombre
                    : $"{a.Nombre} - {a.Marca}",

                                DescripcionCorta = string.IsNullOrWhiteSpace(a.Descripcion)
                    ? ""
                    : a.Descripcion.Length > 60
                        ? a.Descripcion.Substring(0, 60) + "..."
                        : a.Descripcion,

                                DescripcionCompleta = a.Descripcion,

                                Stock = a.Stock ?? 0,

                                PrecioCompra = a.PrecioCompra ?? 0,

                                // AGREGAR ESTO
                                PrecioVentaMinorista = a.PrecioVentaMinorista ?? 0,

                                // OPCIONAL
                                PrecioVentaMayorista = a.PrecioVentaMayorista ?? 0,

                                a.UnidadMedida,

                                a.Proveedor,

                                a.Activo,

                                Imagen = a.Imagen != null
                    ? $"/img/articulos/{a.Imagen}"
                    : null
                });

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // ===========================================
        // EndPoints de categoria y unidad medida
        // ===========================================

        [HttpGet]
        public async Task<IActionResult> ObtenerCategorias()
        {
            try
            {
                var lista = await _context.Categoria
                    .OrderBy(c => c.Nombre)
                    .Select(c => new
                    {
                        c.Id,
                        c.Nombre
                    })
                    .ToListAsync();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUnidadesMedida()
        {
            try
            {
                var lista = await _context.Umedida
                    .OrderBy(u => u.Nombre)
                    .Select(u => new
                    {
                        u.Id,
                        u.Nombre
                    })
                    .ToListAsync();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProveedores()
        {
            try
            {
                var lista = await _context.Proveedors
                    .Include(p => p.IdpersonaNavigation)
                    .OrderBy(p => p.IdpersonaNavigation.Nombres)
                    .Select(p => new
                    {
                        id = p.Id,

                        nombre = p.IdpersonaNavigation != null
                            ? p.IdpersonaNavigation.Nombres
                            : "Proveedor sin nombre"
                    })
                    .ToListAsync();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // =================
        // CREAR ARTICULO
        // =================

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ArticuloVM avm)
        {
            if (avm == null)
            {
                return BadRequest(new
                {
                    message = "Datos vacíos."
                });
            }

            if (string.IsNullOrWhiteSpace(avm.Nombre))
            {
                return BadRequest(new
                {
                    message = "El nombre del artículo es obligatorio."
                });
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // =====================================
                // PROVEEDOR POR DEFECTO
                // =====================================

                int? proveedorId = avm.Idproveedor;

                if (proveedorId == null)
                {
                    proveedorId = await _context.Proveedors
                        .Where(p =>
                            p.IdpersonaNavigation.Nombres == "Ferreteria")
                        .Select(p => p.Id)
                        .FirstOrDefaultAsync();

                    if (proveedorId == 0)
                    {
                        proveedorId = null;
                    }
                }

                // =====================================
                // CREAR ARTICULO
                // =====================================

                Articulo nuevoArticulo = new Articulo
                {
                    Nombre = avm.Nombre.Trim(),

                    Descripcion =
                        string.IsNullOrWhiteSpace(avm.Descripcion)
                            ? null
                            : avm.Descripcion.Trim(),

                    Stock = avm.Stock,

                    Idumedida = avm.Idumedida,

                    Activo = avm.Activo ?? true,

                    Sku = string.IsNullOrWhiteSpace(avm.Sku)
                        ? null
                        : avm.Sku.Trim(),

                    Marca = string.IsNullOrWhiteSpace(avm.Marca)
                        ? null
                        : avm.Marca.Trim(),

                    PrecioCompra = avm.PrecioCompra,

                    PrecioVentaMinorista =
                        avm.PrecioVentaMinorista,

                    PrecioVentaMayorista =
                        avm.PrecioVentaMayorista,

                    StockMinimo = avm.StockMinimo,

                    Ubicacion = string.IsNullOrWhiteSpace(avm.Ubicacion)
                        ? null
                        : avm.Ubicacion.Trim(),

                    Idproveedor = proveedorId,

                    PermiteDescuento =
                        avm.PermiteDescuento ?? false,

                    EsServicio =
                        avm.EsServicio ?? false,

                    PorcentajeGananciaMinorista =
                        avm.PorcentajeGananciaMinorista,

                    PorcentajeGananciaMayorista =
                        avm.PorcentajeGananciaMayorista,

                    PermiteDecimal =
                        avm.PermiteDecimal ?? false
                };

                _context.Articulos.Add(nuevoArticulo);

                await _context.SaveChangesAsync();

                // =====================================
                // CATEGORIAS
                // =====================================

                if (avm.CategoriasId != null &&
                    avm.CategoriasId.Any())
                {
                    var categorias =
                        avm.CategoriasId
                        .Distinct()
                        .Select(c => new Articulocategorium
                        {
                            Idarticulo = nuevoArticulo.Id,
                            Idcategoria = c
                        });

                    await _context.Articulocategoria
                        .AddRangeAsync(categorias);
                }

                // =====================================
                // IMAGENES
                // =====================================

                if (avm.ImagenesBase64 != null &&
                    avm.ImagenesBase64.Any())
                {
                    string rootPath = Path.Combine(
                        _env.WebRootPath,
                        "img",
                        "articulos"
                    );

                    if (!Directory.Exists(rootPath))
                    {
                        Directory.CreateDirectory(rootPath);
                    }

                    foreach (var base64 in avm.ImagenesBase64)
                    {
                        if (string.IsNullOrWhiteSpace(base64))
                            continue;

                        try
                        {
                            var cleanBase64 = base64.Contains(",")
                                ? base64.Split(',')[1]
                                : base64;

                            byte[] bytes =
                                Convert.FromBase64String(cleanBase64);

                            string extension = ".jpg";

                            if (base64.Contains("image/png"))
                                extension = ".png";

                            if (base64.Contains("image/webp"))
                                extension = ".webp";

                            string fileName =
                                $"{Guid.NewGuid()}{extension}";

                            string fullPath =
                                Path.Combine(rootPath, fileName);

                            await System.IO.File
                                .WriteAllBytesAsync(fullPath, bytes);

                            var imagen = new Imagen
                            {
                                Nombre = fileName,

                                Ruta = $"/img/articulos/{fileName}",

                                Tipo = extension,

                                Tamaño = bytes.Length / 1024,

                                Fechapublicada = DateTime.Now,

                                Estado = true
                            };

                            _context.Imagens.Add(imagen);

                            await _context.SaveChangesAsync();

                            var relacion = new Articuloimagen
                            {
                                Idarticulo = nuevoArticulo.Id,
                                Idimagen = imagen.Id
                            };

                            _context.Articuloimagens
                                .Add(relacion);
                        }
                        catch
                        {
                            // ignorar imagen dañada
                            continue;
                        }
                    }
                }

                // =====================================
                // GUARDAR TODO
                // =====================================

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Artículo creado correctamente",
                    id = nuevoArticulo.Id
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return BadRequest(new
                {
                    message = ex.Message
                });
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

                        a.Stock,

                        a.Activo,

                        a.Idumedida,

                        a.Sku,

                        a.Marca,

                        a.PrecioCompra,

                        a.PrecioVentaMinorista,

                        a.PrecioVentaMayorista,

                        a.StockMinimo,

                        a.Ubicacion,

                        a.Idproveedor,

                        a.PermiteDescuento,

                        a.EsServicio,

                        a.PorcentajeGananciaMinorista,

                        a.PorcentajeGananciaMayorista,

                        a.PermiteDecimal,

                        // =====================
                        // PROVEEDOR
                        // =====================

                        Proveedor =
                            a.IdproveedorNavigation != null
                            ? a.IdproveedorNavigation
                                .IdpersonaNavigation.Nombres
                            : "Ferreteria Lago",

                        // =====================
                        // CATEGORIAS
                        // =====================

                        Categorias = _context.Articulocategoria
                            .Where(ac => ac.Idarticulo == a.Id)
                            .Select(ac => new
                            {
                                ac.Idcategoria,
                                ac.IdcategoriaNavigation.Nombre
                            })
                            .ToList(),

                        // =====================
                        // IMAGENES
                        // =====================

                        Imagenes = _context.Articuloimagens
                            .Where(ai => ai.Idarticulo == a.Id)
                            .Join(
                                _context.Imagens,
                                ai => ai.Idimagen,
                                im => im.Id,
                                (ai, im) => new
                                {
                                    im.Id,
                                    im.Ruta,
                                    im.Nombre,
                                    im.Fechapublicada
                                }
                            )
                            .OrderByDescending(x => x.Fechapublicada)
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (articulo == null)
                {
                    return NotFound(new
                    {
                        message = "Artículo no encontrado"
                    });
                }

                return Ok(articulo);
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
        // ACTUALIZAR ARTICULO
        // =======================

        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] ArticuloVM avm)
        {
            if (avm == null)
            {
                return BadRequest(new
                {
                    message = "Datos inválidos."
                });
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

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

                // =====================================
                // PROVEEDOR DEFAULT
                // =====================================

                int? proveedorId = avm.Idproveedor;

                if (proveedorId == null)
                {
                    proveedorId = await _context.Proveedors
                        .Where(p =>
                            p.IdpersonaNavigation.Nombres == "Ferreteria")
                        .Select(p => p.Id)
                        .FirstOrDefaultAsync();

                    if (proveedorId == 0)
                    {
                        proveedorId = null;
                    }
                }

                // =====================================
                // ACTUALIZAR DATOS
                // =====================================

                articulo.Nombre = avm.Nombre?.Trim();

                articulo.Descripcion =
                    string.IsNullOrWhiteSpace(avm.Descripcion)
                        ? null
                        : avm.Descripcion.Trim();

                articulo.Stock = avm.Stock;

                articulo.Idumedida = avm.Idumedida;

                articulo.Activo = avm.Activo ?? true;

                articulo.Sku =
                    string.IsNullOrWhiteSpace(avm.Sku)
                        ? null
                        : avm.Sku.Trim();

                articulo.Marca =
                    string.IsNullOrWhiteSpace(avm.Marca)
                        ? null
                        : avm.Marca.Trim();

                articulo.PrecioCompra =
                    avm.PrecioCompra;

                articulo.PrecioVentaMinorista =
                    avm.PrecioVentaMinorista;

                articulo.PrecioVentaMayorista =
                    avm.PrecioVentaMayorista;

                articulo.StockMinimo =
                    avm.StockMinimo;

                articulo.Ubicacion =
                    string.IsNullOrWhiteSpace(avm.Ubicacion)
                        ? null
                        : avm.Ubicacion.Trim();

                articulo.Idproveedor =
                    proveedorId;

                articulo.PermiteDescuento =
                    avm.PermiteDescuento ?? false;

                articulo.EsServicio =
                    avm.EsServicio ?? false;

                articulo.PorcentajeGananciaMinorista =
                    avm.PorcentajeGananciaMinorista;

                articulo.PorcentajeGananciaMayorista =
                    avm.PorcentajeGananciaMayorista;

                articulo.PermiteDecimal =
                    avm.PermiteDecimal ?? false;

                // =====================================
                // ACTUALIZAR CATEGORIAS
                // =====================================

                var categoriasActuales =
                    _context.Articulocategoria
                    .Where(ac => ac.Idarticulo == articulo.Id);

                _context.Articulocategoria
                    .RemoveRange(categoriasActuales);

                if (avm.CategoriasId != null &&
                    avm.CategoriasId.Any())
                {
                    var nuevasCategorias =
                        avm.CategoriasId
                        .Distinct()
                        .Select(c => new Articulocategorium
                        {
                            Idarticulo = articulo.Id,
                            Idcategoria = c
                        });

                    await _context.Articulocategoria
                        .AddRangeAsync(nuevasCategorias);
                }

                // =====================================
                // NUEVAS IMAGENES
                // =====================================

                if (avm.ImagenesBase64 != null &&
                    avm.ImagenesBase64.Any())
                {
                    string rootPath = Path.Combine(
                        _env.WebRootPath,
                        "img",
                        "articulos"
                    );

                    if (!Directory.Exists(rootPath))
                    {
                        Directory.CreateDirectory(rootPath);
                    }

                    foreach (var base64 in avm.ImagenesBase64)
                    {
                        if (string.IsNullOrWhiteSpace(base64))
                            continue;

                        try
                        {
                            var cleanBase64 =
                                base64.Contains(",")
                                ? base64.Split(',')[1]
                                : base64;

                            byte[] bytes =
                                Convert.FromBase64String(cleanBase64);

                            string extension = ".jpg";

                            if (base64.Contains("image/png"))
                                extension = ".png";

                            if (base64.Contains("image/webp"))
                                extension = ".webp";

                            string fileName =
                                $"{Guid.NewGuid()}{extension}";

                            string fullPath =
                                Path.Combine(rootPath, fileName);

                            await System.IO.File
                                .WriteAllBytesAsync(fullPath, bytes);

                            var imagen = new Imagen
                            {
                                Nombre = fileName,

                                Ruta =
                                    $"/img/articulos/{fileName}",

                                Tipo = extension,

                                Tamaño = bytes.Length / 1024,

                                Fechapublicada = DateTime.Now,

                                Estado = true
                            };

                            _context.Imagens.Add(imagen);

                            await _context.SaveChangesAsync();

                            var relacion = new Articuloimagen
                            {
                                Idarticulo = articulo.Id,
                                Idimagen = imagen.Id
                            };

                            _context.Articuloimagens
                                .Add(relacion);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

                // =====================================
                // GUARDAR
                // =====================================

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Artículo actualizado correctamente"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

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
        public async Task<IActionResult> CambiarEstado(
            int id,
            bool activo)
        {
            try
            {
                var result =
                    await _arServ.CambiarEstado(id, activo);

                if (!result)
                {
                    return BadRequest(new
                    {
                        message =
                            "No se pudo cambiar el estado del artículo."
                    });
                }

                return Ok(new
                {
                    message = activo
                        ? "Artículo activado correctamente."
                        : "Artículo desactivado correctamente."
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

