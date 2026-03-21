using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Descuento
{
    public int Id { get; set; }

    public int? Idcliente { get; set; }

    public int? Idarticulo { get; set; }

    public decimal? Porcentaje { get; set; }

    public virtual Articulo? IdarticuloNavigation { get; set; }

    public virtual Persona? IdclienteNavigation { get; set; }
}
