using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class DetalleFactura
{
    public int Id { get; set; }

    public int? Idfactura { get; set; }

    public int? Idarticulo { get; set; }

    public decimal? Cantidad { get; set; }

    public decimal? Precio { get; set; }

    public virtual Articulo? IdarticuloNavigation { get; set; }

    public virtual Factura? IdfacturaNavigation { get; set; }
}
