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
                            .FirstOrDefault()

                    })
                    .ToListAsync();

                var resultado = new List<Object>();

                foreach (var a in lista)
                {
                    string? base64 = null;

                    // ====================================
                    // String a Base64(imagen)
                    // ====================================
                    if(a.Imagen != null)
                    {
                        try
                        {

                            var base64Raw = await _imgServ
                                .ConvertToBase64String("imagen-articulo", a.Imagen.Nombre);

                            string ext = Path.GetExtension(a.Imagen.Nombre).Replace(".", "");

                            base64 = $"data:image/{ext};base64,{base64Raw}";

                        }
                        catch
                        {
                            base64 = null;
                        }
                    }

                    // ==========================
                    // Acortamos descripción
                    // ==========================
                    string descripcioncorta = "";

                    if (!string.IsNullOrEmpty(a.Descripcion))
                    {
                        descripcioncorta = a.Descripcion.Length > 60
                            ? a.Descripcion.Substring(0, 60) + "..."
                            : a.Descripcion;
                    }

                    resultado.Add(new
                    {
                        a.Id,
                        a.Nombre,
                        DescripcionCorta = descripcioncorta,
                        DescripcionCompleta = a.Descripcion,
                        a.Precio,
                        a.Stock,
                        a.Activo,
                        Umedidum = a.Umedidum,
                        Imagen = base64
                    });
                }

                return Ok(resultado);

            }
            catch (Exception ex)
            {
                throw new Exception($"Error al intentar listar la tabla articulo, tipo de error: {ex.Message}");
            }
            
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
                            Ruta = $"/img/articulos-img/{FName}",
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

    }
}
