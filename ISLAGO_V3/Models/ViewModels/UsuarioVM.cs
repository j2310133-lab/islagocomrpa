using ISLAGO_V3.Models.ViewModels;

public class UsuarioVM
{
    public int Id { get; set; }

    public string Usarname { get; set; } = null!;

    public string? Password { get; set; }

    public string Email { get; set; } = null!;

    public bool? Activo { get; set; }

    public bool? Bloqueado { get; set; }

    public int? IntentosFallidos { get; set; }

    public DateTime? UltimoLogin { get; set; }

    // Persona
    public int? PersonaId { get; set; }

    public List<PersonaVM>? Personas { get; set; }

    // Roles
    public List<int>? RolesSeleccionados { get; set; }

    public List<RolVM>? Roles { get; set; }

    // Imagen
    public string? ImagenBase64 { get; set; }

    public string? ImagenUrl { get; set; }
}