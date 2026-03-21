using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class SesionUsuario
{
    public int Id { get; set; }

    public int? Idusuario { get; set; }

    public string? Token { get; set; }

    public string? Ip { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaExpiracion { get; set; }

    public bool? Activo { get; set; }

    public virtual Usuario? IdusuarioNavigation { get; set; }
}
