using ISLAGO_V3.Datos.Interfaces;
using ISLAGO_V3.Entidad.Models;
using ISLAGO_V3.Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ISLAGO_V3.Negocio.Implementaciones
{
    public class ArticuloServices : IArticuloServices
    {

        private readonly IGenericRepository<Articulo> _artRep;
        private readonly IGenericRepository<Articuloimagen> _AImg;
        private readonly IBase64IMGSercies _imgServ;
        private readonly IUnitOfWork _uow;
        private readonly IGenericRepository<Imagen> _imgRep;
        private readonly IGenericRepository<MovimientoInventario> _mvinv;

        public ArticuloServices(
            IGenericRepository<Articulo> grep,
            IGenericRepository<Articuloimagen> aImg,
            IBase64IMGSercies imgServ,
            IUnitOfWork uow,
            IGenericRepository<Imagen> imgRep,
            IGenericRepository<MovimientoInventario> mvinv)
        {
            _artRep = grep;
            _AImg = aImg;
            _imgServ = imgServ;
            _uow = uow;
            _imgRep = imgRep;
            _mvinv = mvinv;
        }

        private Stream Base64ToStream(string base64)
        {
            try
            {
                var clean = base64.Contains(",")
                    ? base64.Split(',')[1]
                    : base64;

                var bytes = Convert.FromBase64String(clean);
                return new MemoryStream(bytes);
            }
            catch
            {
                throw new Exception("La cadena proporcionada no es una imagen válida en formato Base64.");
            }
        }

        private string ObtenerExtenciones(string base64Ex)
        {
            if (base64Ex.Contains("image/jpeg")) return ".jpg";
            if (base64Ex.Contains("image/png")) return ".png";
            if (base64Ex.Contains("image/svg+xml")) return ".svg";

            return ".jpg"; // Extensión por defecto
        }

        public async Task<Articulo> Crear(Articulo e, List<string>? imagenesBase64)
        {

            using var transaction = await _uow.BeginTransactionAsync();

            try
            {

                // ---------------------
                // VALIDACIONES
                // _____________________

                if (e == null)
                    throw new Exception("El articulo no puede ser nulo.");

                if (string.IsNullOrWhiteSpace(e.Nombre))
                    throw new Exception("El nombre del articulo es obligatorio.");

                if (e.Precio <= 0)
                    throw new Exception("El precio debe de ser mayor que 0.");

                if (e.Stock < 0)
                    throw new Exception("El stock no puede tener sumas negativas");

                // Validar duplicados
                var existencia = await _artRep.Obtener(a => a.Nombre.ToLower() == e.Nombre.ToLower().Trim());
                if (existencia != null)
                    throw new Exception("Ya existe un articulo con ese nombre, por favor elija otro nombre.");

                // ---------------------
                // NORMALIZACION
                // _____________________

                e.Nombre = e.Nombre.Trim();
                e.Descripcion = e.Descripcion?.Trim();
                e.Activo = true;

                // ---------------------
                // CREAR ARTICULO
                // _____________________

                var newart = await _artRep.Crear(e);

                if (newart == null) throw new Exception("No se pudo crear el articulo.");

                // ---------------------
                // IMAGENES
                // _____________________

                if(imagenesBase64 != null && imagenesBase64.Any())
                {
                    int index = 0;

                    foreach (var base64 in imagenesBase64)
                    {
                        if (string.IsNullOrWhiteSpace(base64))
                            continue;

                        // Convertir BASE64 a Stream
                        var stream = Base64ToStream(base64);

                        // obtener extencion
                        var extencion = ObtenerExtenciones(base64);

                        // Nombre unico
                        string NombreArchivo = $"{Guid.NewGuid()}{extencion}";

                        // Guardar imagen y devolver URL
                        var url = await _imgServ.GuardarImagen(stream, "imagen-articulo", NombreArchivo);

                        if(url.StartsWith("Error") || url.StartsWith("No existe")) throw new Exception(url);

                        // Crear registro de imagen en la base de datos
                        var imagen = await _imgRep.Crear(new Imagen
                        {
                            Nombre = NombreArchivo,
                            Ruta = url,
                            Tipo = extencion,
                            Tamaño = (int)(stream.Length / 1024),
                            Fechapublicada = DateTime.UtcNow,
                            Estado = true
                        });

                        if (imagen == null) throw new Exception("No se puede guardar la imagen.");

                        // crear la relacion de imagen con articulo
                        await _AImg.Crear(new Articuloimagen
                        {
                            Idarticulo = newart.Id,
                            Idimagen = imagen.Id
                        });

                        index++;
                    }
                }

                await transaction.CommitAsync();

                return newart;

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al intentar crear el articulo: {ex.Message}", ex);
            }

        }

        public async Task<bool> Actualizar(int id, Articulo e, List<string>? imagenesBase64)
        {
            
            using var transaction = _uow.BeginTransactionAsync();

            try
            {
                if (e == null) throw new Exception("El articulo no puede ser nulo.");

                var articuloDB = await _artRep.Obtener(a => a.Id == id);
                if (articuloDB == null) throw new Exception("No existe el articulo");

                if (string.IsNullOrEmpty(e.Nombre)) throw new Exception("El nombre del articulo es obligatorio.");

                if (e.Precio <= 0) throw new Exception("El precio debe de ser mayor que 0.");

                if (e.Stock < 0) throw new Exception("El stock no puede tener sumas negativas");

                //validar duplicados 
                var duplicado = await _artRep.Obtener(a =>
                    a.Nombre.ToLower() == e.Nombre.ToLower().Trim() &&
                    a.Id != id);

                if (duplicado != null) throw new Exception("Ya existe un articulo con ese nombre.");

                // =========================
                // ACTUALIZAR CAMPOS
                // =========================

                articuloDB.Nombre = e.Nombre.Trim();
                articuloDB.Descripcion = e.Descripcion?.Trim();
                articuloDB.Precio = e.Precio;
                articuloDB.Stock = e.Stock;
                articuloDB.Idumedida = e.Idumedida;
                articuloDB.Activo = e.Activo;

                var result = await _artRep.Editar(articuloDB);

                if (!result) throw new Exception("No se puede actualizar el articulo.");

                return true;
            }
            catch (Exception ex)
            {
                await transaction.Result.RollbackAsync();
                throw new Exception($"Error al intentar actualizar el articulo: {ex.Message}", ex);
            }
                        
        }

        public async Task<bool> AgregarImagenes(int idArt, List<string> base64imagenes)
        {
            using var transaction = await _uow.BeginTransactionAsync();

            try
            {
                var articulo = await _artRep.Obtener(a => a.Id == idArt);

                if (articulo == null) throw new Exception("No existe el articulo.");

                if (base64imagenes == null || !base64imagenes.Any())
                    throw new Exception("No se proporcionaron imágenes para agregar.");

                foreach (var base64 in base64imagenes)
                {
                    if (string.IsNullOrWhiteSpace(base64))
                        continue;

                    var stream = Base64ToStream(base64);
                    var ext = ObtenerExtenciones(base64);
                    string NombreArchivo = $"{Guid.NewGuid()}{ext}";

                    var url = await _imgServ.GuardarImagen(stream, "imagen-articulo", NombreArchivo);

                    if(url.StartsWith("Error") || url.StartsWith("No existe")) 
                        throw new Exception(url);

                    var imagen = await _imgRep.Crear(new Imagen
                    {
                        Nombre = NombreArchivo,
                        Ruta = url,
                        Tipo = ext,
                        Tamaño = (int)(stream.Length / 1024),
                        Fechaeditada = DateTime.UtcNow,
                        Estado = true
                    });

                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al intentar agregar las imagenes: {ex.Message}", ex);
            }
        }

        public async Task<bool> AjustarStock(int idArt, decimal cantidad, string tipo, string motivo)
        {
            using var transaction = await _uow.BeginTransactionAsync();

            try
            {
                // ==================
                // VALIDACIONES
                // ==================

                var articulo = await _artRep.Obtener(a => a.Id == idArt);

                if (articulo == null) throw new Exception("No existe el articulo.");

                if (cantidad <= 0) throw new Exception("La cantidad debe ser mayor que 0.");

                if (string.IsNullOrWhiteSpace(tipo)) throw new Exception("El tipo de movimiento es obligatorio.");

                tipo = tipo.ToUpper().Trim();

                if (tipo != "ENTRADA" && tipo != "SALIDA" && tipo != "AJUSTE")
                    throw new Exception("El tipo de movimiento debe ser 'ENTRADA', 'SALIDA' o 'AJUSTE'.");

                // =======================
                // Aplicar movimientos
                // =======================
                decimal stockAnterior = articulo.Stock ?? 0;
                decimal stockNuevo = stockAnterior;

                if(tipo == "ENTRADA")
                {
                    stockNuevo += cantidad;
                }
                else if (tipo == "SALIDA")
                {
                    if (stockAnterior < cantidad)
                        throw new Exception("Stock insuficiente...");

                    stockNuevo -= cantidad;
                }
                else if(tipo == "AJUSTE")
                {
                    stockNuevo = cantidad;
                }

                // =======================
                // REGISTRAR MOVIMIENTO
                // =======================

                await _mvinv.Crear(new MovimientoInventario
                {
                    Idarticulo = idArt,
                    Tipo = tipo,
                    Cantidad = cantidad,
                    Motivo = motivo?.Trim(),
                    Fecha = DateTime.UtcNow,
                });

                await transaction.CommitAsync();
                return true;

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al intentar ajustar el stock: {ex.Message}", ex);
            }
        }

        public Task<bool> AsignarCategorias(int idArt, List<int> categoriasId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Articulo>> Buscar(string filtro)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CambiarEstado(int id, bool activo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Articulo>> ObtenerActivos()
        {
            throw new NotImplementedException();
        }

        public Task<Articulo> ObtenerPorId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Articulo>> ObtenerPorNombre(string nombre)
        {
            throw new NotImplementedException();
        }

        public Task<List<Articulo>> ObtenerTodos()
        {
            throw new NotImplementedException();
        }
    }
}
