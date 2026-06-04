using ISLAGO_V3.Datos;
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

        public async Task<List<Usuario>> ObtenerTodos()
        {
            var lista = await _repository.Consultar();
            return lista.ToList();
        }

        public async Task<Usuario> ObtenerPorId(int id)
        {
            using var t = await _uow.BeginTransactionAsync();

            try
            {
                // ==========================
                // OBTENER USUARIO
                // ==========================

                var usuario = await _repository.Obtener(u => u.Id == id);

                if (usuario == null) throw new Exception("No existe el usuario  o no fue encontrado");

                // ==========================
                // CARGAR IMAGENES
                // ==========================

                var relImgs = (await _userImgRep.Consultar(ui => ui.Idusuario == id)).ToList();

                var imagenes = new List<Imagen>();

                foreach (var rel in relImgs)
                {
                    var img = await _imgRep.Obtener(
                        i => i.Id == rel.Idimagen
                    );

                    if (img != null) imagenes.Add(img);
                }

                usuario.Usuarioimagens = relImgs;

                // ==========================
                // CARGAR ROLES
                // ==========================

                var roles = await _c.UsuarioRols
                    .Where(r => r.Idusuario == id)
                    .ToListAsync();

                usuario.UsuarioRols = roles;

                // ==========================
                // COMMIT
                // ==========================

                await t.CommitAsync();

                return usuario;
            }
            catch (Exception ex)
            {
                await t.RollbackAsync();

                throw new Exception(
                    $"Error al intentar obtener el usuario por ID: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            using var t = await _uow.BeginTransactionAsync();

            try
            {
                // ==========================
                // BUSCAR USUARIO
                // ==========================

                var usuario = await _repository.Obtener(u => u.Id == id);

                if (usuario == null)
                    throw new Exception("El usuario no existe.");

                // ==========================
                // SOFT DELETE
                // ==========================

                usuario.Activo = false;

                usuario.Bloqueado = true;

                // ==============================
                // CERRAR SESIONES (EN proceso)
                // ==============================

                /*
                var sesiones = await _c.SesionUsuarios
                    .Where(s => s.Idusuario == id)
                    .ToListAsync();

                foreach(var s in sesiones)
                {
                    s.Activa = false;
                }
                */

                // ==========================
                // GUARDAR CAMBIOS
                // ==========================

                _c.Usuarios.Update(usuario);

                await _c.SaveChangesAsync();

                await t.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await t.RollbackAsync();

                throw new Exception(
                    $"Error al eliminar usuario: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<bool> UsernameExiste(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                    return false;

                username = username.Trim().ToLower();

                return await _c.Usuarios.AnyAsync(u =>
                    u.Usarname.ToLower() == username
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error al validar username: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<bool> EmailExiste(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                email = email.Trim().ToLower();

                return await _c.Usuarios.AnyAsync(u =>
                    u.Email.ToLower() == email
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error al validar email: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<bool> CambiarEstado(int id, bool activo)
        {
            using var t = await _uow.BeginTransactionAsync();

            try
            {
                var usuario = await _repository.Obtener(u => u.Id == id);

                if (usuario == null)
                    throw new Exception("Usuario no encontrado.");

                usuario.Activo = activo;

                // si se desactiva -> bloquear
                if (!activo)
                    usuario.Bloqueado = true;

                _c.Usuarios.Update(usuario);

                await _c.SaveChangesAsync();

                await t.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await t.RollbackAsync();

                throw new Exception(
                    $"Error al cambiar estado: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<bool> BloquearUsuario(int id)
        {
            using var t = await _uow.BeginTransactionAsync();

            try
            {
                var usuario = await _repository.Obtener(u => u.Id == id);

                if (usuario == null)
                    throw new Exception("Usuario no encontrado.");

                usuario.Bloqueado = true;

                _c.Usuarios.Update(usuario);

                await _c.SaveChangesAsync();

                await t.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await t.RollbackAsync();

                throw new Exception(
                    $"Error al bloquear usuario: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<bool> DesbloquearUsuario(int id)
        {
            using var t = await _uow.BeginTransactionAsync();

            try
            {
                var usuario = await _repository.Obtener(u => u.Id == id);

                if (usuario == null)
                    throw new Exception("Usuario no encontrado.");

                usuario.Bloqueado = false;

                usuario.IntentosFallidos = 0;

                _c.Usuarios.Update(usuario);

                await _c.SaveChangesAsync();

                await t.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await t.RollbackAsync();

                throw new Exception(
                    $"Error al desbloquear usuario: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<List<Usuario>> ObtenerActivos()
        {
            try
            {
                return await _c.Usuarios
                    .Where(u => u.Activo == true)
                    .Include(u => u.UsuarioRols)
                    .Include(u => u.Usuarioimagens)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error al obtener usuarios activos: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<List<Usuario>> ObtenerBloqueados()
        {
            try
            {
                return await _c.Usuarios
                    .Where(u => u.Bloqueado == true)
                    .Include(u => u.UsuarioRols)
                    .Include(u => u.Usuarioimagens)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error al obtener usuarios bloqueados: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<List<Usuario>> ObtenerPorUsername(string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return new List<Usuario>();

                nombre = nombre.Trim().ToLower();

                return await _c.Usuarios
                    .Where(u =>
                        u.Usarname.ToLower().Contains(nombre)
                    )
                    .Include(u => u.UsuarioRols)
                    .Include(u => u.Usuarioimagens)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error al buscar por username: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<List<Usuario?>> ObtenerPorGmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return new List<Usuario?>();

                email = email.Trim().ToLower();

                return await _c.Usuarios
                    .Where(u =>
                        u.Email.ToLower().Contains(email)
                    )
                    .Include(u => u.UsuarioRols)
                    .Include(u => u.Usuarioimagens)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error al buscar por email: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<List<Usuario>> Buscar(string filtro)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filtro))
                    return new List<Usuario>();

                filtro = filtro.Trim().ToLower();

                return await _c.Usuarios

                    .Include(u => u.UsuarioRols)

                    .Include(u => u.Usuarioimagens)

                    .Include(u => u.IdpersonaNavigation)

                    .Where(u =>

                        u.Usarname.ToLower().Contains(filtro)

                        || u.Email.ToLower().Contains(filtro)

                        || (
                            u.IdpersonaNavigation != null
                            && (
                                u.IdpersonaNavigation.Nombres.ToLower().Contains(filtro)
                            )
                        )
                    )

                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error al buscar usuarios: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<bool> ValidarPassword(Usuario usuario, string password
            )
        {
            try
            {
                if (usuario == null)
                    throw new Exception("Usuario inválido.");

                if (string.IsNullOrWhiteSpace(password))
                    return false;

                var resultado = _hasher.VerifyHashedPassword(
                    usuario,
                    usuario.PasswordHash,
                    password
                );

                return resultado == PasswordVerificationResult.Success;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error al validar contraseña: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<bool> CambiarContrasenia(int idUser,  string actualpass, string newpass)
        {
            using var t = await _uow.BeginTransactionAsync();

            try
            {
                // ==========================
                // BUSCAR USUARIO
                // ==========================

                var usuario = await _repository.Obtener(
                    u => u.Id == idUser
                );

                if (usuario == null)
                    throw new Exception("Usuario no encontrado.");

                // ==========================
                // VALIDAR ESTADO
                // ==========================

                if (usuario.Activo != true)
                    throw new Exception("La cuenta está inactiva.");

                if (usuario.Bloqueado == true)
                    throw new Exception("La cuenta está bloqueada.");

                // ==========================
                // VALIDAR PASSWORD ACTUAL
                // ==========================

                bool passCorrecta = await ValidarPassword(
                    usuario,
                    actualpass
                );

                if (!passCorrecta)
                    throw new Exception("La contraseña actual es incorrecta.");

                // ==========================
                // VALIDAR DIFERENTE
                // ==========================

                if (actualpass == newpass)
                    throw new Exception(
                        "La nueva contraseña no puede ser igual a la anterior."
                    );

                // ==========================
                // ACTUALIZAR PASSWORD
                // ==========================

                usuario.PasswordHash = _hasher.HashPassword(usuario, newpass);

                _c.Usuarios.Update(usuario);

                await _c.SaveChangesAsync();

                await t.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await t.RollbackAsync();

                throw new Exception(
                    $"Error al cambiar contraseña: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<Usuario?> Login(string username, string password)
        {
            using var t = await _uow.BeginTransactionAsync();

            try
            {
                if (
                    string.IsNullOrWhiteSpace(username)
                    || string.IsNullOrWhiteSpace(password)
                )
                {
                    throw new Exception(
                        "Debe ingresar usuario y contraseña."
                    );
                }

                username = username.Trim().ToLower();

                // ==========================
                // BUSCAR USUARIO
                // ==========================

                var usuario = await _c.Usuarios

                    .Include(u => u.UsuarioRols)

                    .Include(u => u.Usuarioimagens)

                    .Include(u => u.IdpersonaNavigation)

                    .FirstOrDefaultAsync(u =>

                        u.Usarname.ToLower() == username

                        || u.Email.ToLower() == username
                    );

                if (usuario == null)
                    throw new Exception("Usuario no encontrado.");

                // ==========================
                // VALIDAR ESTADO
                // ==========================

                if (usuario.Activo != true)
                    throw new Exception("Cuenta inactiva.");

                if (usuario.Bloqueado == true)
                    throw new Exception("Cuenta bloqueada.");

                // ==========================
                // VALIDAR PASSWORD
                // ==========================

                bool passwordValida =
                    await ValidarPassword(usuario, password);

                if (!passwordValida)
                {
                    usuario.IntentosFallidos =
                        (usuario.IntentosFallidos ?? 0) + 1;

                    // bloquear al llegar a 5
                    if (usuario.IntentosFallidos >= 5)
                    {
                        usuario.Bloqueado = true;
                    }

                    _c.Usuarios.Update(usuario);

                    await _c.SaveChangesAsync();

                    throw new Exception("Contraseña incorrecta.");
                }

                // ==========================
                // RESETEAR INTENTOS
                // ==========================

                usuario.IntentosFallidos = 0;

                usuario.UltimoLogin = DateTime.UtcNow;

                _c.Usuarios.Update(usuario);

                await _c.SaveChangesAsync();

                await t.CommitAsync();

                return usuario;
            }
            catch (Exception ex)
            {
                await t.RollbackAsync();

                throw new Exception(
                    $"Error al iniciar sesión: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<bool> ResetPassword(
    int iduser,
    bool activo
)
        {
            using var t = await _uow.BeginTransactionAsync();

            try
            {
                var usuario = await _repository.Obtener(
                    u => u.Id == iduser
                );

                if (usuario == null)
                    throw new Exception("Usuario no encontrado.");

                // password temporal
                string temporal = "Temp123*";

                usuario.PasswordHash =
                    _hasher.HashPassword(usuario, temporal);

                usuario.Activo = activo;

                usuario.IntentosFallidos = 0;

                usuario.Bloqueado = false;

                _c.Usuarios.Update(usuario);

                await _c.SaveChangesAsync();

                await t.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await t.RollbackAsync();

                throw new Exception(
                    $"Error al resetear contraseña: {ex.Message}",
                    ex
                );
            }
        }

        public async Task<bool> LogOut(string token)
        {
            try
            {
                // futuro:
                // invalidar token
                // cerrar sesion
                // blacklist JWT

                await Task.CompletedTask;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error al cerrar sesión: {ex.Message}",
                    ex
                );
            }
        }
    }
}
