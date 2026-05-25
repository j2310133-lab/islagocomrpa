using ISLAGO_V3.Entidad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLAGO_V3.Negocio.Interfaces
{
    public interface IAuthServices
    {

        Task<Usuario?> Login(
            string username,
            string password,
            string ip
        );

        Task<bool> Logout(string token);

        Task<bool> ValidarPassword(
            string password,
            string hash
        );

        Task<string> GenerarToken(int idUsuario);
    }

}