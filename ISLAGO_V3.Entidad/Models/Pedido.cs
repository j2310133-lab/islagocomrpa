using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Pedido
{
    public int Id { get; set; }

    public int? Idcliente { get; set; }

    public string? NombreClienteTemp { get; set; }

    public string? TelefonoTemp { get; set; }

    public DateTime? Fecha { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public string? Estado { get; set; }

    public string? Prioridad { get; set; }

    public decimal? Total { get; set; }

    public decimal? TotalPagado { get; set; }

    public decimal? Deuda { get; set; }

    public int? Idusuario { get; set; }

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    public virtual ICollection<Devolucion> Devolucions { get; set; } = new List<Devolucion>();

    public virtual ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual Persona? IdclienteNavigation { get; set; }

    public virtual Usuario? IdusuarioNavigation { get; set; }

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
