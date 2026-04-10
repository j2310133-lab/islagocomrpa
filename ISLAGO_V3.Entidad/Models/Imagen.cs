using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Imagen
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string Ruta { get; set; } = null!;

    public string? Tipo { get; set; }

    public int? Tamaño { get; set; }

    public DateTime? Fechapublicada { get; set; }

    public DateTime? Fechaeditada { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<Articuloimagen> Articuloimagens { get; set; } = new List<Articuloimagen>();

    public virtual ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual ICollection<Proforma> Proformas { get; set; } = new List<Proforma>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
