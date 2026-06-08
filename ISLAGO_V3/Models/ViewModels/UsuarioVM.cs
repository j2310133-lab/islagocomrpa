using ISLAGO_V3.Entidad.Models;

namespace ISLAGO_V3.Models.ViewModels
{
    public class UsuarioVM
    {
        public int Id { get; set; }

        public string Usarname { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string Email { get; set; } = null!;

        public bool? Activo { get; set; }

        public bool? Bloqueado { get; set; }

        public int? IntentosFallidos { get; set; }

        public DateTime? UltimoLogin { get; set; }

        //persona
        public List<int>? PersonaId { get; set; }
        public List<PersonaVM>? Personas { get; set; }

        // imagenes
        public List<string>? ImagenesBase64 { get; set; }
        public string? ImagenesUrl { get; set; }
    }
}
