using ISLAGO_V3.Entidad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLAGO_V3.Negocio.Interfaces
{
    public interface IArticuloServices
    {
        // Consultas 
        Task<List<Articulo>> ObtenerTodos(); //Este es el READ Del CRUD(Create, Read, Update and Delete)
        Task<Articulo> ObtenerPorId(int id);
        Task<List<Articulo>> ObtenerPorNombre(string nombre);
        Task<List<Articulo>> Buscar(string filtro);
        Task<List<Articulo>> ObtenerActivos();

        // CRUD
        Task<Articulo> Crear(Articulo e, List<string>? imagenesBase64);
        Task<bool> Actualizar(int id, Articulo e, List<string>? imagenesBase64);
        Task<bool> Eliminar(int id);

        // Estado
        Task<bool> CambiarEstado(int id, bool activo);

        // Inventario 
        Task<bool> AjustarStock(int idArt, decimal cantidad, string tipo, string motivo);

        // Relaciones
        Task<bool> AsignarCategorias(int idArt, List<int> categoriasId);
        Task<bool> AgregarImagenes(int idArt, List<string> base64imagenes);
    }
}
