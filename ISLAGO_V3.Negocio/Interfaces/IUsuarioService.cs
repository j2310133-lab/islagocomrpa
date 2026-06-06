using ISLAGO_V3.Entidad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLAGO_V3.Negocio.Interfaces
{
    public interface IUsuarioService
    {
        Task<List<Usuario>> ObtenerTodos();
        Task<Usuario> ObtenerPorId(int id);
        Task<List<Usuario>> ObtenerPorUsername(string nombre);
        Task<List<Usuario?>> ObtenerPorEmail(string email);
        Task<List<Usuario>> Buscar(string filtro);
        Task<List<Usuario>> ObtenerActivos();
        Task<List<Usuario>> ObtenerBloqueados();

        Task<Usuario?> ObtenerUnoPorUsername(string username);
        Task<Usuario?> ObtenerUnoPorEmail(string email);

        Task<List<Rol>> ObtenerRoles(int idUsuario);

        // ===================================
        // Logica de autenticaciòn
        // ===================================
        Task<Usuario?> Login(string username, string password);
        Task<bool> LogOut(string token);
        Task<bool> ValidarPassword(Usuario usuario, string passwrod);

        // CRUD
        Task<Usuario> Crear(Usuario  user, string password,List<int> roles, string? imagenBase64);
        Task<bool> Actualizar(int id,Usuario user, string password, List<int> roles, string? imagenBase64);
        Task<bool> Eliminar(int id);
        // =====================
        // PASSWORD
        // =====================
        Task<bool> CambiarContrasenia(int idUser, string actualpass, string newpass);
        Task<bool> ResetPassword(int iduser, bool activo);

        // =================================
        // Logica Estado de usuario
        // =================================
        Task<bool> CambiarEstado(int id, bool activo);
        Task<bool> BloquearUsuario(int id);
        Task<bool> DesbloquearUsuario(int id);

        // =====================
        // VALIDACIONES
        // =====================
        Task<bool> UsernameExiste(string username);

        Task<bool> EmailExiste(string email);

        Task<bool> AsignarRoles(int idUsuario, List<int> roles);

        Task<bool> IncrementarIntentosFallidos(int idUsuario);
        Task<bool> ReiniciarIntentosFallidos(int idUsuario);

        Task<List<SesionUsuario>> ObtenerSesionesActivas(int idUsuario);
        Task<bool> CerrarTodasLasSesiones(int idUsuario);

        Task<string> GenerarTokenRecuperacion(int idUsuario);
        Task<bool> ValidarTokenRecuperacion(string token);
        Task<bool> RecuperarPassword(string token, string nuevaPassword);

        Task<bool> Activar2FA(int idUsuario);
        Task<bool> Desactivar2FA(int idUsuario);
        Task<string> GenerarCodigo2FA(int idUsuario);
        Task<bool> ValidarCodigo2FA(int idUsuario, string codigo);
    }
}
