using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Notificacion
{
    public int Id { get; set; }

    public int? Idusuario { get; set; }

    public string? Mensaje { get; set; }

    public string? Tipo { get; set; }

    public bool? Leido { get; set; }

    public DateTime? Fecha { get; set; }

    public virtual Usuario? IdusuarioNavigation { get; set; }
}
