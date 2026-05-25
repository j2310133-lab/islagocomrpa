using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Historialestadopedido
{
    public int Id { get; set; }

    public int? Idpedido { get; set; }

    public int? Idestadopedido { get; set; }

    public DateTime? Fecha { get; set; }

    public virtual Estadopedido? IdestadopedidoNavigation { get; set; }

    public virtual Pedido? IdpedidoNavigation { get; set; }
}
