using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Umedidum
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Articulo> Articulos { get; set; } = new List<Articulo>();
}
