using ISLAGO_V3.Entidad.Models;
using ISLAGO_V3.Entidad.Models.Options;
using ISLAGO_V3.Negocio.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLAGO_V3.Negocio.Implementaciones
{
    public class Base64IMGServices : IBase64IMGSercies
    {
        private readonly StorageOptions _opt;

        public Base64IMGServices(IOptions<StorageOptions> options)
        {
            _opt = options.Value;
        }

        public async Task<string> GuardarImagen(Stream archivoStream, string tipoRecurso, string nombreArchivo)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(tipoRecurso) || !_opt.RutasBase.ContainsKey(tipoRecurso))
                {
                    return $"No existe configuracion para el recurso : {tipoRecurso}";
                }

                //Get BaseRoute
                string rutaBase = _opt.RutasBase[tipoRecurso];

                //Size of validation
                long maxBytes = long.MaxValue;
                if(_opt.TamañosMaximosMb != null && _opt.TamañosMaximosMb.ContainsKey(tipoRecurso))
                {
                    maxBytes = (long)_opt.TamañosMaximosMb[tipoRecurso] * 1024 * 1024; // Convertir MB a Bytes
                }

                //Extension validation
                string[] formatos = Array.Empty<string>();
                if (_opt.FormatosPermitidos != null && _opt.FormatosPermitidos.ContainsKey(tipoRecurso)) formatos = _opt.FormatosPermitidos[tipoRecurso];

                //Normalize stream (in case it's not seekable)
                Stream streamToSave = archivoStream;
                long length;
                if (archivoStream.CanSeek)
                {
                    length = archivoStream.Length;
                    archivoStream.Seek(0, SeekOrigin.Begin);    
                }
                else
                {
                    var ms = new MemoryStream();
                    await archivoStream.CopyToAsync(ms);
                    length = ms.Length;
                    ms.Position = 0;
                    streamToSave = ms;
                }

                if(length > maxBytes) return $"El tamaño del archivo excede el límite permitido de {maxBytes / (1024 * 1024)} MB.";

                if (formatos.Length > 0)
                {
                    string ext = Path.GetExtension(nombreArchivo)?.TrimStart('.').ToLowerInvariant() ?? string.Empty;
                    if (!formatos.Any(f => string.Equals(f, ext, StringComparison.OrdinalIgnoreCase)))
                    {
                        return $"El formato {ext} no esta permitido para el recurso '{tipoRecurso}'. ";
                    }
                }

                Directory.CreateDirectory(rutaBase);

                string nombreSeguro = Path.GetFileName(nombreArchivo);
                string rutaFisica = Path.Combine(rutaBase, nombreSeguro);

                if (streamToSave.CanSeek) streamToSave.Seek(0, SeekOrigin.Begin);

                using (var Fs = new FileStream(rutaFisica, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await streamToSave.CopyToAsync(Fs);
                }

                string PublicBase = (_opt.PublicBaseUrl ?? string.Empty).TrimEnd('/');
                string url = string.IsNullOrWhiteSpace(PublicBase)
                    ? $"/{tipoRecurso}/{nombreSeguro}"
                    : $"{PublicBase}/{tipoRecurso}/{nombreSeguro}";

                return url;

            }
            catch (Exception e)
            {
                throw new Exception($"Error al intentar guardar la imagen: {e.Message}");
            }
        }
        public async Task<string> ActualizarImagen(Stream archivoStream, string tipoRecurso, string nombreArchivoAntiguo, string nombreArchivoNuevo, string extension)
        {
            try
            {

                if (archivoStream == null || !archivoStream.CanRead || archivoStream.Length == 0)
                    throw new ArgumentException("El stream de la imagen es inválido", nameof(archivoStream));

                if (archivoStream.CanSeek) archivoStream.Position = 0;

                if (string.IsNullOrWhiteSpace(extension)) extension = ".jpg"; 

                if (!extension.StartsWith(".")) extension = "." + extension;

                // New name
                string newName = string.IsNullOrWhiteSpace(nombreArchivoNuevo)
                     ? $"{Guid.NewGuid()}{extension}"
                     : $"{nombreArchivoNuevo}{extension}";

                string url = await GuardarImagen(archivoStream, tipoRecurso, newName);

                // Eliminar anterior si existe
                if (!string.IsNullOrWhiteSpace(nombreArchivoAntiguo))
                {
                    await EliminarStorage(tipoRecurso, nombreArchivoAntiguo);
                }

                return url;

            }
            catch (Exception e)
            {
                throw new Exception($"Error al intentar actualizar la imagen: {e.Message}");
            }
        }

        public async Task<string> ConvertToBase64String(string tipoRecurso, string nombreArchivo)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(tipoRecurso) || !_opt.RutasBase.ContainsKey(tipoRecurso))
                    throw new Exception($"No existe configuracion para el recurso: {tipoRecurso}");

                string rutaBase = _opt.RutasBase[tipoRecurso];
                string rutaFisica = Path.Combine(rutaBase, nombreArchivo);

                if (!File.Exists(rutaFisica))
                    throw new FileNotFoundException("El archivo no existe en la ruta especificada.", rutaFisica);

                byte[] fileBytes = await File.ReadAllBytesAsync(rutaFisica);
                string base64String = Convert.ToBase64String(fileBytes);
                return base64String;

            }
            catch (Exception e)
            {
                throw new Exception($"Error al intentar convertir la imagen a Base64: {e.Message}");
            }
        }

        public Task<string> EliminarStorage(string tipoRecurso, string nombreArchivo)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(tipoRecurso) || !_opt.RutasBase.ContainsKey(tipoRecurso))
                    return Task.FromResult($"Configuration not found for resource: {tipoRecurso}");

                if (string.IsNullOrEmpty(nombreArchivo)) return Task.FromResult($"Nombre de archivo no valido: {nombreArchivo}");

                string rb = _opt.RutasBase[tipoRecurso];
                string ns = Path.GetFileName(nombreArchivo);
                string rf = Path.Combine(rb, ns);

                if (!File.Exists(rf))
                {
                    return Task.FromResult($"This file no exists: {rf}");
                }

                File.Delete(rf);

                return Task.FromResult("Archivo Eliminado con exito.");

            }
            catch (Exception e)
            {
                throw new Exception($"Error al intentar eliminar la imagen: {e.Message}");
            }
        }
    }
}
