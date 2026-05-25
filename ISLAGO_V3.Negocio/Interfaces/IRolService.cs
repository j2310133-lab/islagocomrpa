using ISLAGO_V3.Entidad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLAGO_V3.Negocio.Interfaces
{
    public interface IRolService
    {

        Task<List<Rol>> ObtenerTodos();

        Task<Rol?> ObtenerPorId(int id);

        Task<Rol?> ObtenerPorNombre(string nombre);

        Task<bool> Crear(Rol rol);

        Task<bool> Actualizar(int id, Rol rol);

        Task<bool> Eliminar(int id);

        // =========================
        // USUARIO - ROL
        // =========================

        Task<List<Rol>> ObtenerRolesUsuario(int idUsuario);

        Task<bool> AsignarRol(int idUsuario,int idRol);

        Task<bool> RemoverRol(int idUsuario, int idRol);

        Task<bool> UsuarioTieneRol(int idUsuario, string rol);

    }
}
