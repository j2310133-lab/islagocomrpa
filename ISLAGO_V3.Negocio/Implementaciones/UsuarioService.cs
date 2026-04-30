using ISLAGO_V3.Entidad.Models;
using ISLAGO_V3.Negocio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLAGO_V3.Negocio.Implementaciones
{
    public class UsuarioService : IUsuarioService
    {
        public Task<Usuario> ObtenerPorId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Usuario>> ObtenerTodos()
        {
            throw new NotImplementedException();
        }
    }
}
