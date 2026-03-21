using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Usuario2fauth
{
    public int Id { get; set; }

    public int? Idusuario { get; set; }

    public string? Secreto { get; set; }

    public bool? Activo { get; set; }

    public virtual Usuario? IdusuarioNavigation { get; set; }
}
