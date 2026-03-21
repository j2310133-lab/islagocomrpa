using System;
using System.Collections.Generic;

namespace ISLAGO_V3.Entidad.Models;

public partial class TipoPersona
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();
}
