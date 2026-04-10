using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Articulocategorium
{
    public int Id { get; set; }

    public int? Idarticulo { get; set; }

    public int? Idcategoria { get; set; }

    public virtual Articulo? IdarticuloNavigation { get; set; }

    public virtual Categorium? IdcategoriaNavigation { get; set; }
}
