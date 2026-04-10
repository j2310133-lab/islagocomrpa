using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Articulo
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal? Stock { get; set; }

    public decimal Precio { get; set; }

    public int? Idumedida { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Articulocategorium> Articulocategoria { get; set; } = new List<Articulocategorium>();

    public virtual ICollection<Articuloimagen> Articuloimagens { get; set; } = new List<Articuloimagen>();

    public virtual ICollection<Descuento> Descuentos { get; set; } = new List<Descuento>();

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; }    = new List<DetalleFactura>();

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    public virtual ICollection<DetalleProforma> DetalleProformas { get; set; } = new List<DetalleProforma>();

    public virtual ICollection<Devolucion> Devolucions { get; set; } = new List<Devolucion>();

    public virtual Umedidum? IdumedidaNavigation { get; set; }

    public virtual ICollection<MovimientoInventario> MovimientoInventarios { get; set; } = new List<MovimientoInventario>();
}
