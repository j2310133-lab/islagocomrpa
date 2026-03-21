using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Compra
{
    public int Id { get; set; }

    public int? Idproveedor { get; set; }

    public DateTime? Fecha { get; set; }

    public decimal? Total { get; set; }

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual Proveedor? IdproveedorNavigation { get; set; }
}
