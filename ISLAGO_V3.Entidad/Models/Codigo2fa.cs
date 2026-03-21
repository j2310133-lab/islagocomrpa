using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Codigo2fa
{
    public int Id { get; set; }

    public int? Idusuario { get; set; }

    public string? Codigo { get; set; }

    public DateTime? Expiracion { get; set; }

    public bool? Usado { get; set; }

    public virtual Usuario? IdusuarioNavigation { get; set; }
}
