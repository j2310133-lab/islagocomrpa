using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Usarname { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool? Activo { get; set; }

    public bool? Bloqueado { get; set; }

    public int? IntentosFallidos { get; set; }

    public DateTime? UltimoLogin { get; set; }

    public int? Idpersona { get; set; }

    public virtual ICollection<Codigo2fa> Codigo2fas { get; set; } = new List<Codigo2fa>();

    public virtual Persona? IdpersonaNavigation { get; set; }

    public virtual ICollection<Notificacion> Notificacions { get; set; } = new List<Notificacion>();

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    public virtual ICollection<Proforma> Proformas { get; set; } = new List<Proforma>();

    public virtual ICollection<RecuperacionCuentum> RecuperacionCuenta { get; set; } = new List<RecuperacionCuentum>();

    public virtual ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();

    public virtual ICollection<SesionUsuario> SesionUsuarios { get; set; } = new List<SesionUsuario>();

    public virtual ICollection<Usuario2fauth> Usuario2fauths { get; set; } = new List<Usuario2fauth>();

    public virtual ICollection<UsuarioRol> UsuarioRols { get; set; } = new List<UsuarioRol>();

    public virtual ICollection<Usuarioimagen> Usuarioimagens { get; set; } = new List<Usuarioimagen>();
}
