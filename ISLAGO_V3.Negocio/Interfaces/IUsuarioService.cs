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
        Task<List<Usuario?>> ObtenerPorGmail(string email);
        Task<List<Usuario>> Buscar(string filtro);
        Task<List<Usuario>> ObtenerActivos();
        Task<List<Usuario>> ObtenerBloqueados();

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
    }
}
