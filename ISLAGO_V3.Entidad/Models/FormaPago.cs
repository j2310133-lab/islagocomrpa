using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class FormaPago
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
