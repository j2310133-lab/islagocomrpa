using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Usuarioimagen
{
    public int Id { get; set; }

    public int Idusuario { get; set; }

    public int Idimagen { get; set; }

    public bool? EsPrincipal { get; set; }

    public int? Orden { get; set; }

    public virtual Imagen IdimagenNavigation { get; set; } = null!;

    public virtual Usuario IdusuarioNavigation { get; set; } = null!;
}
