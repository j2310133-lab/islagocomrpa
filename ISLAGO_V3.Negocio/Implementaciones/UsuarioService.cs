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
            IUnitOfWork uow)
        {
            _c = contexto;
            _hasher = hasher;

            _imgServ = imgServ;
            _imgRep = imgRep;
            _userImgRep = userImgRep;
            _uow = uow;
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
                throw new Exception($"Error al intentar crear un usuario");
            }
        }

        public Task<bool> Actualizar(int id, Usuario user, string password, List<int> roles, string? imagenBase64)
        {
            
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

        public Task<List<Usuario>> ObtenerTodos()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResetPassword(int iduser, bool activo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UsernameExiste(string username)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidarPassword(Usuario usuario, string passwrod)
        {
            throw new NotImplementedException();
        }
    }
}
