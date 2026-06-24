using ISLAGO_V3.Entidad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLAGO_V3.Negocio.Interfaces
{
    public interface ICompraService
    {
        Task<List<Compra>> ObtenerTodas();

        Task<bool> Crear(
            Compra compra,
            List<DetalleCompra> detalles);
    }
}