using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class UsuarioRol
{
    public int Id { get; set; }

    public int? Idusuario { get; set; }

    public int? Idrol { get; set; }

    public virtual Rol? IdrolNavigation { get; set; }

    public virtual Usuario? IdusuarioNavigation { get; set; }
}
