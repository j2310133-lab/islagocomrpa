using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLAGO_V3.Entidad.Models.Options
{
    public class StorageOptions
    {
        public Dictionary<string, string> RutaBase { get; set; } = new();
        public Dictionary<string, string[]> FormatosPermitidos { get; set; } = new();
        public Dictionary<string, int> TamañosMaximosMB { get; set; } = new();
        public string PublicBaseURL { get; set; }
    }
}
