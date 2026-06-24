using ISLAGO_V3.Datos.DBContext;
using ISLAGO_V3.Entidad.Models;
using ISLAGO_V3.Negocio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ISLAGO_V3.Datos.Implementaciones
{
    public class CompraService : ICompraService
    {
        private readonly DBContextISLAGO _context;

        public CompraService(DBContextISLAGO context)
        {
            _context = context;
        }

        public async Task<List<Compra>> ObtenerTodas()
        {
            return await _context.Compras
                .Include(c => c.IdproveedorNavigation)
                .ThenInclude(p => p.IdpersonaNavigation)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();
        }

        public async Task<bool> Crear(
            Compra compra,
            List<DetalleCompra> detalles)
        {
            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                compra.Fecha = DateTime.Now;

                await _context.Compras.AddAsync(compra);

                await _context.SaveChangesAsync();

                foreach (var detalle in detalles)
                {
                    detalle.Idcompra = compra.Id;

                    await _context.DetalleCompras.AddAsync(detalle);

                    var articulo = await _context.Articulos
                        .FirstOrDefaultAsync(a =>
                            a.Id == detalle.Idarticulo);

                    if (articulo != null)
                    {
                        articulo.Stock =
                            (articulo.Stock ?? 0) +
                            (detalle.Cantidad ?? 0);
                    }
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }
    }
}