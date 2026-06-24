namespace ISLAGO_V3.Models.ViewModels
{
    public class RolVM
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        // Para CheckBox o MultiSelect
        public bool Seleccionado { get; set; }

        // Estadísticas futuras
        public int CantidadUsuarios { get; set; }

        // Para mostrar información adicional
        public string? Descripcion { get; set; }
    }
}