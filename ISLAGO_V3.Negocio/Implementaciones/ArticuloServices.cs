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
        private readonly IGenericRepository<Articulocategorium> _artCatRep;

        public ArticuloServices(
            IGenericRepository<Articulo> grep,
            IGenericRepository<Articuloimagen> aImg,
            IBase64IMGSercies imgServ,
            IUnitOfWork uow,
            IGenericRepository<Imagen> imgRep,
            IGenericRepository<MovimientoInventario> mvinv,
            IGenericRepository<Articulocategorium> artCatRep)
        {
            _artRep = grep;
            _AImg = aImg;
            _imgServ = imgServ;
            _uow = uow;
            _imgRep = imgRep;
            _mvinv = mvinv;
            _artCatRep = artCatRep;
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

                if (e.PrecioCompra < 0)
                    throw new Exception("El precio de compra no puede ser negativo.");

                if (e.PrecioVentaMinorista < 0)
                    throw new Exception("El precio minorista no puede ser negativo.");

                if (e.PrecioVentaMayorista < 0)
                    throw new Exception("El precio mayorista no puede ser negativo.");

                if ((e.Stock ?? 0) < 0)
                    throw new Exception("El stock no puede ser negativo.");

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
                e.Marca = e.Marca?.Trim();
                e.Sku = e.Sku?.Trim().ToUpper();
                e.Ubicacion = e.Ubicacion?.Trim();

                if (e.Idproveedor == null || e.Idproveedor <= 0)
                {
                    e.Idproveedor = 1;
                }

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

            using var transaction = await _uow.BeginTransactionAsync();

            try
            {

                // =========================
                // VALIDACIONES
                // =========================

                if (e == null)
                    throw new Exception("El articulo no puede ser nulo.");

                var articuloDB = await _artRep.Obtener(a => a.Id == id);

                if (articuloDB == null)
                    throw new Exception("No existe el articulo.");

                if (string.IsNullOrWhiteSpace(e.Nombre))
                    throw new Exception("El nombre del articulo es obligatorio.");

                if (e.PrecioCompra < 0)
                    throw new Exception("El precio de compra no puede ser negativo.");

                if (e.PrecioVentaMinorista < 0)
                    throw new Exception("El precio minorista no puede ser negativo.");

                if (e.PrecioVentaMayorista < 0)
                    throw new Exception("El precio mayorista no puede ser negativo.");

                if ((e.Stock ?? 0) < 0)
                    throw new Exception("El stock no puede ser negativo.");

                // =========================
                // VALIDAR DUPLICADOS
                // =========================

                var duplicado = await _artRep.Obtener(a =>
                    a.Nombre.ToLower() == e.Nombre.ToLower().Trim()
                    && a.Id != id
                );

                if (duplicado != null)
                    throw new Exception("Ya existe un articulo con ese nombre.");

                // =========================
                // NORMALIZAR
                // =========================

                e.Nombre = e.Nombre.Trim();

                e.Descripcion = e.Descripcion?.Trim();

                e.Marca = e.Marca?.Trim();

                e.Sku = e.Sku?.Trim().ToUpper();

                e.Ubicacion = e.Ubicacion?.Trim();

                // =========================
                // PROVEEDOR POR DEFECTO
                // =========================

                if (e.Idproveedor == null || e.Idproveedor <= 0)
                {
                    e.Idproveedor = 1;
                }

                // =========================
                // ACTUALIZAR CAMPOS
                // =========================

                articuloDB.Nombre = e.Nombre;
                articuloDB.Descripcion = e.Descripcion;

                articuloDB.Stock = e.Stock;

                articuloDB.Idumedida = e.Idumedida;

                articuloDB.Activo = e.Activo;

                articuloDB.Sku = e.Sku;

                articuloDB.Marca = e.Marca;

                articuloDB.PrecioCompra = e.PrecioCompra;

                articuloDB.PrecioVentaMinorista = e.PrecioVentaMinorista;

                articuloDB.PrecioVentaMayorista = e.PrecioVentaMayorista;

                articuloDB.StockMinimo = e.StockMinimo;

                articuloDB.Ubicacion = e.Ubicacion;

                articuloDB.Idproveedor = e.Idproveedor;

                articuloDB.PermiteDescuento = e.PermiteDescuento;

                articuloDB.EsServicio = e.EsServicio;

                articuloDB.PorcentajeGananciaMinorista =
                    e.PorcentajeGananciaMinorista;

                articuloDB.PorcentajeGananciaMayorista =
                    e.PorcentajeGananciaMayorista;

                articuloDB.PermiteDecimal = e.PermiteDecimal;

                // =========================
                // GUARDAR CAMBIOS
                // =========================

                var result = await _artRep.Editar(articuloDB);

                if (!result)
                    throw new Exception("No se puede actualizar el articulo.");

                // =========================
                // AGREGAR NUEVAS IMAGENES
                // =========================

                if (imagenesBase64 != null && imagenesBase64.Any())
                {

                    foreach (var base64 in imagenesBase64)
                    {

                        if (string.IsNullOrWhiteSpace(base64))
                            continue;

                        // BASE64 -> STREAM
                        var stream = Base64ToStream(base64);

                        // EXTENSION
                        var extencion = ObtenerExtenciones(base64);

                        // NOMBRE ARCHIVO
                        string NombreArchivo =
                            $"{Guid.NewGuid()}{extencion}";

                        // GUARDAR STORAGE
                        var url = await _imgServ.GuardarImagen(
                            stream,
                            "imagen-articulo",
                            NombreArchivo
                        );

                        if (url.StartsWith("Error")
                            || url.StartsWith("No existe"))
                        {
                            throw new Exception(url);
                        }

                        // GUARDAR IMAGEN DB
                        var imagen = await _imgRep.Crear(new Imagen
                        {
                            Nombre = NombreArchivo,
                            Ruta = url,
                            Tipo = extencion,
                            Tamaño = (int)(stream.Length / 1024),
                            Fechaeditada = DateTime.UtcNow,
                            Estado = true
                        });

                        if (imagen == null)
                            throw new Exception("No se pudo guardar la imagen.");

                        // RELACION
                        await _AImg.Crear(new Articuloimagen
                        {
                            Idarticulo = articuloDB.Id,
                            Idimagen = imagen.Id
                        });

                    }

                }

                await transaction.CommitAsync();

                return true;

            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();

                throw new Exception(
                    $"Error al intentar actualizar el articulo: {ex.Message}",
                    ex
                );

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

                    await _AImg.Crear(new Articuloimagen
                    {
                        Idarticulo = idArt,
                        Idimagen = imagen.Id
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

                // ACTUALIZAR STOCK
                articulo.Stock = stockNuevo;

                var actualizado = await _artRep.Editar(articulo);

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

        public async Task<bool> AsignarCategorias(int idArt, List<int> categoriasId)
        {

            using var t = await _uow.BeginTransactionAsync();

            try
            {

                var ar = await _artRep.Obtener(a => a.Id == idArt);

                if(ar == null) throw new Exception("No existe el articulo.");

                if (categoriasId == null || !categoriasId.Any())
                    throw new Exception("No se proporcionaron categorias para asignar.");

                // ================================
                // ELIMINAR RELACIONES ACTUALES
                // ================================

                var RA = await _artCatRep.Consultar();

                var listaEliminar = RA.Where(ac => ac.Idarticulo == idArt).ToList();

                foreach(var rel in listaEliminar)
                {
                    await _artCatRep.Eliminar(rel);
                }

                // =========================
                // Insertar nuevas
                // =========================

                foreach(var catID in categoriasId.Distinct())
                {
                    await _artCatRep.Crear(new Articulocategorium
                    {
                        Idarticulo = idArt,
                        Idcategoria = catID
                    });
                }

                await t.CommitAsync();
                return true;

            }
            catch (Exception ex)
            {
                await t.RollbackAsync();
                throw new Exception($"Error al intentar asignar las categorias: {ex.Message}", ex);
            }

        }

        public async Task<Articulo> ObtenerPorId(int id)
        {
            using var t = await _uow.BeginTransactionAsync();

            try
            {

                var articulo = await _artRep.Obtener(a => a.Id == id);

                if (articulo == null) throw new Exception("No existe el articulo.");

                // =================
                // CARGAR IMAGENES
                // =================

                var relImgs = (await _AImg.Consultar(ai => ai.Idarticulo == id)).ToList();

                var img = new List<Imagen>();

                foreach(var rel in relImgs)
                {
                    var i = await _imgRep.Obtener(im => im.Id == rel.Idimagen);
                    if (i != null) img.Add(i);
                }

                articulo.Articuloimagens = relImgs;

                // ======================
                // CARGAR CATEGORIAS
                // ======================
                var relCats = (await _artCatRep.Consultar(ac => ac.Idarticulo == id)).ToList();

                articulo.Articulocategoria = relCats;

                await t.CommitAsync();
                return articulo;

            }
            catch (Exception ex)
            {
                await t.RollbackAsync();
                throw new Exception($"Error al intentar obtener el articulo: {ex.Message}", ex);
            }
        }

        public async Task<bool> CambiarEstado(int id, bool activo)
        {
            using var transaction = await _uow.BeginTransactionAsync();

            try
            {

                var articulo = await _artRep.Obtener(a => a.Id == id);

                if (articulo == null) throw new Exception("No existe el articulo.");

                articulo.Activo = activo;

                var result = await _artRep.Editar(articulo);    

                if (!result) throw new Exception("No se puede cambiar el estado del articulo.");

                await transaction.CommitAsync();
                return true;

            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al intentar cambiar el estado. {ex.Message}", ex);
            }
        }

        public async Task<List<Articulo>> Buscar(string filtro)
        {
            using var t = await _uow.BeginTransactionAsync();

            try
            {

                if (string.IsNullOrWhiteSpace(filtro)) return new List<Articulo>();

                filtro = filtro.ToLower().Trim();

                var lista = await _artRep.Consultar();

                await t.CommitAsync();

                return lista.Where(a =>
                    a.Nombre.ToLower().Contains(filtro) ||

                    (a.Descripcion != null &&
                     a.Descripcion.ToLower().Contains(filtro)) ||

                    (a.Marca != null &&
                     a.Marca.ToLower().Contains(filtro)) ||

                    (a.Sku != null &&
                     a.Sku.ToLower().Contains(filtro))
                ).ToList();

            }
            catch (Exception ex)
            {
                await t.RollbackAsync();
                throw new Exception($"Error al intentar buscar los articulos: {ex.Message}", ex);
            }
        }

        public async Task<List<Articulo>> ObtenerActivos()
        {
            try
            {

                var lista = await _artRep.Consultar(a => a.Activo == true);
                return lista.ToList();

            }
            catch(Exception ex)
            {
                throw new Exception($"Error al intentar listar activos: {ex.Message}", ex);
            }
        }

        public async Task<List<Articulo>> ObtenerPorNombre(string nombre)
        {
            try
            {

                if(string.IsNullOrWhiteSpace(nombre)) return new List<Articulo>();

                nombre = nombre.ToLower().Trim();

                var lista = await _artRep.Consultar(a =>
                    a.Nombre.ToLower().Contains(nombre)
                    ||
                    (a.Marca != null &&
                     a.Marca.ToLower().Contains(nombre))
                );

                return lista.ToList();

            }
            catch(Exception ex)
            {
                throw new Exception($"Error al intentar obtener por nombre: {ex.Message}", ex);
            }
        }

        public async Task<List<Articulo>> ObtenerTodos()
        {

            try
            {

                var lista = await _artRep.Consultar();
                return lista.ToList();

            }
            catch(Exception ex)
            {
                throw new Exception($"Error al intentar obtener todos los articulos: {ex.Message}", ex);
            }

        }

        public async Task<bool> Eliminar(int id)
        {

            using var t = await _uow.BeginTransactionAsync();

            try
            {

                var articulo = await _artRep.Obtener(a => a.Id == id);
                if (articulo == null) throw new Exception("No existe el articulo o no fue encontrado.");

                // ======================================
                // VALIDAR INVENTARIO
                // ======================================

                var movimientos = await _mvinv.Consultar(m => m.Idarticulo == id);
                if(movimientos.Any()) throw new Exception("No se puede eliminar el articulo porque tiene movimientos de inventario registrados.");

                // ======================================
                // ELIMINAR RELACIONES DE IMAGENES
                // ======================================

                var relImgs = (await _AImg.Consultar(ai => ai.Idarticulo == id)).ToList();
                foreach (var rel in relImgs)
                {
                    await _AImg.Eliminar(rel);

                    // eliminar imagen
                    var img = await _imgRep.Obtener(im => im.Id == rel.Idimagen);
                    if (img != null)
                    {
                        // eliminar archivo fisico
                        await _imgServ.EliminarStorage("imagen-articulo", img.Ruta);

                        // eliminar registro de imagen en base de datos
                        await _imgRep.Eliminar(img);
                    }
                }

                // =======================================
                // ELIMINAR RELACIONES CATEGORIA
                // =======================================

                var relCats = (await _artCatRep.Consultar(ac => ac.Idarticulo == id)).ToList();

                foreach(var relCat in relCats) await _artCatRep.Eliminar(relCat);

                // =======================================
                // ELIMINAR ARTICULO
                // =======================================

                var result = await _artRep.Eliminar(articulo);

                if (!result)  throw new Exception("No se puede eliminar el articulo.");

                await t.CommitAsync();
                return true;

            }
            catch (Exception ex)
            {
                await t.RollbackAsync();
                throw new Exception($"Error al intentar eliminar el articulo: {ex.Message}", ex);
            }

        }

    }
}
