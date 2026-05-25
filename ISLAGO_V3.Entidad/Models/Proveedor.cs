using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Proveedor
{
    public int Id { get; set; }

    public int? Idpersona { get; set; }

    public virtual ICollection<Articulo> Articulos { get; set; } = new List<Articulo>();

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

    public virtual Persona? IdpersonaNavigation { get; set; }
}
