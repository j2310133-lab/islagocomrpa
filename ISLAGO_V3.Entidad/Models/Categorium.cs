using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class Categorium
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int? Idpadre { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Articulocategorium> Articulocategoria { get; set; } = new List<Articulocategorium>();

    public virtual Categorium? IdpadreNavigation { get; set; }

    public virtual ICollection<Categorium> InverseIdpadreNavigation { get; set; } = new List<Categorium>();
}
