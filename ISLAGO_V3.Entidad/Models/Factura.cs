using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Factura
{
    public int Id { get; set; }

    public int? Idpedido { get; set; }

    public string? Numero { get; set; }

    public DateTime? Fecha { get; set; }

    public decimal? Total { get; set; }

    public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();

    public virtual Pedido? IdpedidoNavigation { get; set; }
}
