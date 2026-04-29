using ISLAGO_V3.Entidad.Models;

namespace ISLAGO_V3.Models.ViewModels
{
    public class ArticuloVM
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null;
        public string? Descripcion { get; set; }

        public decimal Precio { get; set; }
        public decimal? Stock { get; set; }

        public int Idumedida { get; set; }

        public bool? Activo { get; set; }

        // Entrada imágenes
        public List<string>? ImagenesBase64 { get; set; }

        // Salida imágenes
        public string? ImagenesUrl { get; set; }

        // Entrada categorías
        public List<int>? CategoriasId { get; set; }

        // Salida categorías 
        public List<CategoriaVM>? Categorias { get; set; }

        // Listar el total de imagenes
        public int TotalImagenes {  get; set; }
    }
}
