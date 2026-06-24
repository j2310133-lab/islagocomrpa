using ISLAGO_V3.Entidad.Models;

namespace ISLAGO_V3.Models.ViewModels
{
    public class CompraVM
    {
        public Compra Compra { get; set; }

        public List<DetalleCompra> Detalles { get; set; }
    }
}