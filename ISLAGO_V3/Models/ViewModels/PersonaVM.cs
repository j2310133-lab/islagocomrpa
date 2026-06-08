using ISLAGO_V3.Entidad.Models;

namespace ISLAGO_V3.Models.ViewModels
{
    public class PersonaVM
    {
        public int Id { get; set; }

        public string Nombres { get; set; } = null!;

        public string Apellidos { get; set; } = null!;

        public string Telefono { get; set; } = null!;

        public string Direccion { get; set; } = null!;

        public bool? Estado { get; set; }

        public int? IdtipoPersona { get; set; }

        public virtual TipoPersona? IdtipoPersonaNavigation { get; set; }

        public virtual Proveedor? Proveedor { get; set; }

        public virtual Usuario? Usuario { get; set; }
    }
}
