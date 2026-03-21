using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Proforma
{
    public int Id { get; set; }

    public int? Idcliente { get; set; }

    public string? NombreClienteTemp { get; set; }

    public string? TelefonoTemp { get; set; }

    public DateTime? Fecha { get; set; }

    public decimal? Total { get; set; }

    public int? Idusuario { get; set; }

    public virtual ICollection<DetalleProforma> DetalleProformas { get; set; } = new List<DetalleProforma>();

    public virtual Persona? IdclienteNavigation { get; set; }

    public virtual Usuario? IdusuarioNavigation { get; set; }
}
