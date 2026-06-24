using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class MovimientoInventario
{
    public int Id { get; set; }

    public int? Idarticulo { get; set; }

    public string? Tipo { get; set; }

    public decimal? Cantidad { get; set; }

    public string? Motivo { get; set; }

    public DateTime? Fecha { get; set; }

    public decimal? StockAnterior { get; set; }

    public decimal? StockNuevo { get; set; }

    public int? Idusuario { get; set; }

    public int? Idcompra { get; set; }

    public int? Idpedido { get; set; }

    public virtual Articulo? IdarticuloNavigation { get; set; }

    public virtual Compra? IdcompraNavigation { get; set; }

    public virtual Pedido? IdpedidoNavigation { get; set; }

    public virtual Usuario? IdusuarioNavigation { get; set; }
}
