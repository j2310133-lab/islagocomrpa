using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Pago
{
    public int Id { get; set; }

    public int? Idpedido { get; set; }

    public int? Idformapago { get; set; }

    public decimal? Monto { get; set; }

    public string? Referencia { get; set; }

    public string? Comprobante { get; set; }

    public DateTime? Fecha { get; set; }

    public virtual FormaPago? IdformapagoNavigation { get; set; }

    public virtual Pedido? IdpedidoNavigation { get; set; }
}
