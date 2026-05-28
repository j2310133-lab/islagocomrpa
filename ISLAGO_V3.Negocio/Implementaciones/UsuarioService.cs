using ISLAGO_V3.Datos.DBContext;
using ISLAGO_V3.Entidad.Models;
using ISLAGO_V3.Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ISLAGO_V3.Datos.Interfaces;

namespace ISLAGO_V3.Negocio.Implementaciones
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _repository;
        private readonly DBContextISLAGO _c;
        private readonly PasswordHasher<Usuario> _hasher;
        private readonly IBase64IMGSercies _imgServ;
        private readonly IGenericRepository<Imagen> _imgRep;
        private readonly IGenericRepository<Usuarioimagen> _userImgRep;
        private readonly IUnitOfWork _uow;

        public UsuarioService(
            DBContextISLAGO contexto,
            PasswordHasher<Usuario> hasher,
            IBase64IMGSercies imgServ,
            IGenericRepository<Imagen> imgRep,
            IGenericRepository<Usuarioimagen> userImgRep,
            IUnitOfWork uow,
            IGenericRepository<Usuario> rep)
        {
            _c = contexto;
            _hasher = hasher;

            _imgServ = imgServ;
            _imgRep = imgRep;
            _userImgRep = userImgRep;
            _uow = uow;
            _repository = rep;
        }

        // =======================
        // HELPERS
        // =======================
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
                throw new Exception("La imagen enviada no es válida.");
            }
        }

        private string ObtenerExtencion(string base64)
        {
            if (base64.Contains("image/jpeg")) return ".jpg";

            if (base64.Contains("image/png")) return ".png";

            if (base64.Contains("image/webp")) return ".webp";

            return ".jpg";
        }
        // =============================
        // FUNCIONALIDADES
        // =============================

        public async Task<List<Usuario>> ObtenerTodos()
        {
            var lista = await _repository.Consultar();
            return lista.ToList();
        }

        public async Task<Usuario> Crear(Usuario user, string password, List<int> roles, string? imagenBase64)
        {
            try
            {
                // ========================
                // VALIDACION DE USERNAME
                // ========================
                bool existUname = await _c.Usuarios.AnyAsync(u => u.Usarname == user.Usarname);

                if (existUname) throw new Exception("El nombre de usuario ya existe");

                // =======================
                // VALIDAR EMAIL
                // =======================
                bool existeEmail = await _c.Usuarios.AnyAsync(u => u.Email == user.Email);

                if (existeEmail) throw new Exception("Este correo ya esta registrado");

                // ======================
                // PASSWORD HASH
                // ======================
                user.PasswordHash = _hasher.HashPassword(user, password);

                // ======================
                // DEFAULT DATA
                // ======================
                user.Activo = true;

                user.Bloqueado = false;

                //user.Fechacreacion = DateTime.Now; -- para el futuro

                // Guardar Usuario
                _c.Usuarios.Add(user);

                await _c.SaveChangesAsync();

                // ======================
                // GUARDAR IMAGEN
                // ======================

                if (!string.IsNullOrWhiteSpace(imagenBase64))
                {
                    // BASE64 -> STREAM
                    var stream = Base64ToStream(imagenBase64);

                    // EXTENSION
                    var ext = ObtenerExtencion(imagenBase64);

                    // NOMBRE UNICO
                    string nombreArchivo = $"{Guid.NewGuid()}{ext}";

                    // GUARDAR STORAGE
                    var url = await _imgServ.GuardarImagen(
                        stream,
                        "imagen-usuario",
                        nombreArchivo
                    );

                    if (url.StartsWith("Error") || url.StartsWith("No existe"))
                        throw new Exception(url);

                    // GUARDAR IMAGEN EN DB
                    var imagen = await _imgRep.Crear(new Imagen
                    {
                        Nombre = nombreArchivo,
                        Ruta = url,
                        Tipo = ext,
                        Tamaño = (int)(stream.Length / 1024),
                        Fechapublicada = DateTime.UtcNow,
                        Estado = true
                    });

                    if (imagen == null)
                        throw new Exception("No se pudo guardar la imagen.");

                    // RELACION USUARIO - IMAGEN
                    await _userImgRep.Crear(new Usuarioimagen
                    {
                        Idusuario = user.Id,
                        Idimagen = imagen.Id
                    });
                }

                // Roles
                if (roles != null && roles.Any())
                {
                    var rolesUsuario = roles
                        .Distinct()
                        .Select(r => new UsuarioRol
                        {
                            Idusuario = user.Id,
                            Idrol = r
                        });

                    await _c.UsuarioRols.AddRangeAsync(rolesUsuario);

                    await _c.SaveChangesAsync();
                }

                return user;
            }
            catch(Exception ex)
            {
                throw new Exception($"Error al intentar crear un usuario: {ex.Message}", ex);
            }
        }

        public async Task<bool> Actualizar(int id, Usuario user, string password, List<int> roles, string? imagenBase64)
        {

            using var t = await _uow.BeginTransactionAsync();

            try
            {

                // ==========================
                // VALIDAR EXISTENCIA
                // ==========================
                var uDB = await _c.Usuarios
                    .Include(u => u.UsuarioRols)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (uDB == null)
                    throw new Exception("El usuario no existe.");

                // ==========================
                // VALIDAR USERNAME
                // ==========================
                bool existeUsername = await _c.Usuarios.AnyAsync(u =>
                    u.Usarname.ToLower() == user.Usarname.ToLower().Trim()
                    && u.Id != id
                );

                if (existeUsername)
                    throw new Exception("El nombre de usuario ya existe.");

                // ==========================
                // VALIDAR EMAIL
                // ==========================
                bool existeEmail = await _c.Usuarios.AnyAsync(u =>
                    u.Email.ToLower() == user.Email.ToLower().Trim()
                    && u.Id != id
                );

                if (existeEmail)
                    throw new Exception("Este correo ya está registrado.");

                // ==========================
                // NORMALIZAR
                // ==========================
                user.Usarname = user.Usarname.Trim();

                user.Email = user.Email.Trim().ToLower();

                // ==========================
                // ACTUALIZAR CAMPOS
                // ==========================
                uDB.Usarname = user.Usarname;

                uDB.Email = user.Email;

                uDB.Activo = user.Activo;

                uDB.Bloqueado = user.Bloqueado;

                uDB.Idpersona = user.Idpersona;

                // ==========================
                // PASSWORD OPCIONAL
                // ==========================
                if (!string.IsNullOrWhiteSpace(password))
                {
                    uDB.PasswordHash =
                        _hasher.HashPassword(uDB, password);
                }

                // ==========================
                // ACTUALIZAR ROLES
                // ==========================
                if (roles != null)
                {

                    // eliminar actuales
                    var rolesActuales = _c.UsuarioRols
                        .Where(r => r.Idusuario == id);

                    _c.UsuarioRols.RemoveRange(rolesActuales);

                    // agregar nuevos
                    var nuevosRoles = roles
                        .Distinct()
                        .Select(r => new UsuarioRol
                        {
                            Idusuario = id,
                            Idrol = r
                        });

                    await _c.UsuarioRols.AddRangeAsync(nuevosRoles);
                }

                // ==========================
                // ACTUALIZAR IMAGEN
                // ==========================
                if (!string.IsNullOrWhiteSpace(imagenBase64))
                {

                    // buscar relacion actual
                    var relActual = await _c.Usuarioimagens
                        .FirstOrDefaultAsync(x => x.Idusuario == id);

                    // ==========================
                    // ELIMINAR IMAGEN ANTERIOR
                    // ==========================
                    if (relActual != null)
                    {

                        var imgAnterior = await _imgRep.Obtener(i =>
                            i.Id == relActual.Idimagen
                        );

                        if (imgAnterior != null)
                        {

                            // eliminar storage
                            await _imgServ.EliminarStorage(
                                "imagen-usuario",
                                imgAnterior.Ruta
                            );

                            // eliminar db imagen
                            await _imgRep.Eliminar(imgAnterior);
                        }

                        // eliminar relacion
                        await _userImgRep.Eliminar(relActual);
                    }

                    // ==========================
                    // NUEVA IMAGEN
                    // ==========================
                    var stream = Base64ToStream(imagenBase64);

                    var ext = ObtenerExtencion(imagenBase64);

                    string nombreArchivo =
                        $"{Guid.NewGuid()}{ext}";

                    var url = await _imgServ.GuardarImagen(
                        stream,
                        "imagen-usuario",
                        nombreArchivo
                    );

                    if (url.StartsWith("Error")
                        || url.StartsWith("No existe"))
                    {
                        throw new Exception(url);
                    }

                    var imagen = await _imgRep.Crear(new Imagen
                    {
                        Nombre = nombreArchivo,
                        Ruta = url,
                        Tipo = ext,
                        Tamaño = (int)(stream.Length / 1024),
                        Fechaeditada = DateTime.UtcNow,
                        Estado = true
                    });

                    if (imagen == null)
                        throw new Exception("No se pudo guardar la imagen.");

                    await _userImgRep.Crear(new Usuarioimagen
                    {
                        Idusuario = id,
                        Idimagen = imagen.Id
                    });

                }

                // ==========================
                // GUARDAR CAMBIOS
                // ==========================
                await _c.SaveChangesAsync();

                await t.CommitAsync();

                return true;

            }
            catch (Exception ex)
            {

                await t.RollbackAsync();

                throw new Exception(
                    $"Error al intentar actualizar el usuario: {ex.Message}",
                    ex
                );

            }

        }

        public Task<bool> UsernameExiste(string username)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BloquearUsuario(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Usuario>> Buscar(string filtro)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CambiarContrasenia(int idUser, string actualpass, string newpass)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CambiarEstado(int id, bool activo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DesbloquearUsuario(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EmailExiste(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Usuario?> Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LogOut(string token)
        {
            throw new NotImplementedException();
        }

        public Task<List<Usuario>> ObtenerActivos()
        {
            throw new NotImplementedException();
        }

        public Task<List<Usuario>> ObtenerBloqueados()
        {
            throw new NotImplementedException();
        }

        public Task<List<Usuario?>> ObtenerPorGmail(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Usuario> ObtenerPorId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Usuario>> ObtenerPorUsername(string nombre)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResetPassword(int iduser, bool activo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidarPassword(Usuario usuario, string passwrod)
        {
            throw new NotImplementedException();
        }
    }
}
