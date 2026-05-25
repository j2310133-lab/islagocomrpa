using ISLAGO_V3.Entidad.Models;

namespace ISLAGO_V3.Models.ViewModels
{
    public class ArticuloVM
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public decimal? Stock { get; set; }

        public int? Idumedida { get; set; }

        public bool? Activo { get; set; }

        public string? Sku { get; set; }

        public string? Marca { get; set; }

        public decimal? PrecioCompra { get; set; }

        public decimal? PrecioVentaMinorista { get; set; }

        public decimal? PrecioVentaMayorista { get; set; }

        public decimal? StockMinimo { get; set; }

        public string? Ubicacion { get; set; }

        public int? Idproveedor { get; set; }

        public bool? PermiteDescuento { get; set; }

        public bool? EsServicio { get; set; }

        public decimal? PorcentajeGananciaMinorista { get; set; }

        public decimal? PorcentajeGananciaMayorista { get; set; }

        public bool? PermiteDecimal { get; set; }

        // IMAGENES
        public List<string>? ImagenesBase64 { get; set; }

        public string? ImagenesUrl { get; set; }

        // CATEGORIAS
        public List<int>? CategoriasId { get; set; }

        public List<CategoriaVM>? Categorias { get; set; }

        public int TotalImagenes { get; set; }
    }
}
