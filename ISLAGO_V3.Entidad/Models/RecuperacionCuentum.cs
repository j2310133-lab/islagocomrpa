using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class RecuperacionCuentum
{
    public int Id { get; set; }

    public int? Idusuario { get; set; }

    public string? Token { get; set; }

    public DateTime? FechaExpiracion { get; set; }

    public bool? Usado { get; set; }

    public virtual Usuario? IdusuarioNavigation { get; set; }
}
