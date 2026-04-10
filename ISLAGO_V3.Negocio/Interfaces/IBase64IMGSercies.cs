using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLAGO_V3.Negocio.Interfaces
{
    public interface IBase64IMGSercies
    {
        Task<string> GuardarImagen(Stream archivoStream, string tipoRecurso, string nombreArchivo);
        Task<string> ConvertToBase64String(string tipoRecurso, string nombreArchivo);

        Task<string> ActualizarImagen(Stream archivoStream, string tipoRecurso, string nombreArchivoAntiguo, string nombreArchivoNuevo, string extension);
        Task<string> EliminarStorage(string tipoRecurso, string nombreArchivo);
    }
}
