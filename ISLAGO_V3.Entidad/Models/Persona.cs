using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Persona
{
    public int Id { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public bool? Estado { get; set; }

    public int? IdtipoPersona { get; set; }

    public virtual ICollection<Descuento> Descuentos { get; set; } = new List<Descuento>();

    public virtual TipoPersona? IdtipoPersonaNavigation { get; set; }

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    public virtual ICollection<Proforma> Proformas { get; set; } = new List<Proforma>();

    public virtual Proveedor? Proveedor { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
