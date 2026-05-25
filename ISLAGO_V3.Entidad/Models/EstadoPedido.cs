using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Estadopedido
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Historialestadopedido> Historialestadopedidos { get; set; } = new List<Historialestadopedido>();

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
