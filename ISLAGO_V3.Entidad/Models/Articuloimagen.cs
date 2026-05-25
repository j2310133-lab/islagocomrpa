using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Articuloimagen
{
    public int Id { get; set; }

    public int? Idimagen { get; set; }

    public int? Idarticulo { get; set; }

    public bool? EsPrincipal { get; set; }

    public int? Orden { get; set; }

    public virtual Articulo? IdarticuloNavigation { get; set; }

    public virtual Imagen? IdimagenNavigation { get; set; }
}
