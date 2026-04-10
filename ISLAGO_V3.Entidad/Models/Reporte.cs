using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Reporte
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string? Tipo { get; set; }

    public string? Ruta { get; set; }

    public DateTime? FechaGenerado { get; set; }

    public DateTime? FechaActualizado { get; set; }

    public int? Idusuario { get; set; }

    public virtual Usuario? IdusuarioNavigation { get; set; }
}
