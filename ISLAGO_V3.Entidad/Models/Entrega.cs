using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Entrega
{
    public int Id { get; set; }

    public int? Idpedido { get; set; }

    public DateTime? FechaReal { get; set; }

    public int? IdimagenEvidencia { get; set; }

    public virtual Imagen? IdimagenEvidenciaNavigation { get; set; }

    public virtual Pedido? IdpedidoNavigation { get; set; }
}
