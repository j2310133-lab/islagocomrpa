using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class DetallePedido
{
    public int Id { get; set; }

    public int? Idpedido { get; set; }

    public int? Idarticulo { get; set; }

    public decimal? Cantidad { get; set; }

    public decimal? Precio { get; set; }

    public decimal? Subtotal { get; set; }

    public virtual Articulo? IdarticuloNavigation { get; set; }

    public virtual Pedido? IdpedidoNavigation { get; set; }
}
