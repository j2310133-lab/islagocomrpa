using ISLAGO_V3.Datos;
using ISLAGO_V3.Entidad.Models;
using ISLAGO_V3.Models.ViewModels;
using ISLAGO_V3.Negocio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace ISLAGO_V3.Controllers
{
    public class CompraController : Controller
    {
        private readonly ICompraService _compraService;
        private readonly DBContextISLAGO _context;

        public CompraController(
            ICompraService compraService,
            DBContextISLAGO context)
        {
            _compraService = compraService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var compras = await _compraService.ObtenerTodas();

            return View(compras);
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var compras = await _compraService.ObtenerTodas();

            var resultado = compras.Select(c => new
            {
                id = c.Id,

                proveedor = c.IdproveedorNavigation != null
                    ? c.IdproveedorNavigation.IdpersonaNavigation.Nombres
                    : "Sin proveedor",

                fecha = c.Fecha?.ToString("dd/MM/yyyy"),

                fechaBusqueda = c.Fecha?.ToString("yyyy-MM-dd"),

                total = c.Total
            });

            return Json(resultado);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerArticulosPorProveedor(int idProveedor)
        {
            var articulos = await _context.Articulos
                .Where(a => a.Idproveedor == idProveedor)
                .Select(a => new
                {
                    id = a.Id,
                    nombre = a.Nombre,
                    precio = a.PrecioCompra
                })
                .ToListAsync();

            return Json(articulos);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProveedores()
        {
            var proveedores = await _context.Proveedors
                .Include(p => p.IdpersonaNavigation)
                .Select(p => new
                {
                    id = p.Id,
                    nombre = p.IdpersonaNavigation != null
                        ? p.IdpersonaNavigation.Nombres
                        : "Sin nombre"
                })
                .ToListAsync();

            return Json(proveedores);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDetalleCompra(int idCompra)
        {
            var detalles = await _context.DetalleCompras
                .Where(d => d.Idcompra == idCompra)
                .Include(d => d.IdarticuloNavigation)
                .ToListAsync();

            var resultado = detalles.Select(d => new
            {
                articulo = d.IdarticuloNavigation.Nombre,
                cantidad = d.Cantidad,
                precio = d.Precio,
                subtotal = d.Subtotal
            });

            return Json(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CompraVM model)
        {
            try
            {
                await _compraService.Crear(
                    model.Compra,
                    model.Detalles);

                return Json(new
                {
                    success = true,
                    message = "Compra registrada correctamente"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DescargarPdf(int id)
        {
            try
            {
                var compra = await _context.Compras
                    .Include(c => c.IdproveedorNavigation)
                        .ThenInclude(p => p.IdpersonaNavigation)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (compra == null)
                    return NotFound();

                var detalles = await _context.DetalleCompras
                    .Where(d => d.Idcompra == id)
                    .Include(d => d.IdarticuloNavigation)
                    .ToListAsync();

                var documento = new Helpers.CompraPdf(compra, detalles);

                var pdf = documento.GeneratePdf();

                return File(pdf,
                    "application/pdf",
                    $"Compra_{compra.Id}.pdf");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }
    }
}