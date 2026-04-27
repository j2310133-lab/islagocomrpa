using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ISLAGO_V3.Models.ViewModels;
using ISLAGO_V3.Entidad.Models;
using ISLAGO_V3.Datos.DBContext;

namespace ISLAGO_V3.Controllers
{
    public class ArticuloController : Controller
    {
        private readonly DBContextISLAGO _context;
        private readonly IWebHostEnvironment _env;

        public ArticuloController(
            DBContextISLAGO c,
            IWebHostEnvironment env)
        {
            _context = c;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        // =================
        // LISTAR ARTICULO
        // =================

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

                // =========================
                // CATEGORIAS
                // =========================
                if (avm.CategoriasId != null && avm.CategoriasId.Any())
                {
                    var relaciones = avm.CategoriasId.Select(CId => new Articulocategorium
                    {
                        Idarticulo = nuevoArticulo.Id,
                        Idcategoria = CId
                    });

                    _context.Articulocategoria.AddRange(relaciones);
                }


            }
            catch(Exception e)
            {
                throw new Exception($"Error al intentar crear el articulo, error lanzado en controlador: {e.Message}", e);
            }
        }

    }
}
