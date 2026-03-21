using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ISLAGO_V3.Datos.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filt = null);
        Task<TEntity> Crear(TEntity e);
        Task<bool> Eliminar(TEntity e);
        Task<bool> Editar(TEntity e);
        Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filt = null);
    }
}
