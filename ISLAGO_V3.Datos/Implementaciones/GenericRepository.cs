using ISLAGO_V3.Datos;
using ISLAGO_V3.Datos.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ISLAGO_V3.Datos.Implementaciones
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DBContextISLAGO _context;

        public GenericRepository(DBContextISLAGO context)
        {
            _context = context;
        }

        public async Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filt = null)
        {
            try
            {
                IQueryable<TEntity> queryEntidad = filt == null ? _context.Set<TEntity>() : _context.Set<TEntity>().Where(filt);
                return queryEntidad;
            }
            catch(Exception ex)
            {
                throw new Exception($"Error al intentar consultar el siguiente objeto: {ex.Message}");
            }
            
        }

        public async Task<TEntity> Crear(TEntity e)
        {
            try
            {

                _context.Set<TEntity>().Add(e);
                await _context.SaveChangesAsync();
                return e;

            }
            catch(Exception ex)
            {
                throw new Exception($"Error al intentar crear el objeto: {ex.Message}");
            }
        }

        public async Task<bool> Editar(TEntity e)
        {
            try
            {

                _context.Set<TEntity>().Update(e);
                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception($"Error al intentar editar el objeto: {ex.Message}");
            }
        }

        public async Task<bool> Eliminar(TEntity e)
        {
            try
            {

                _context.Remove(e);
                await _context.SaveChangesAsync();
                return true;

            }
            catch(Exception ex)
            {
                throw new Exception($"Error al intentar eliminar el objeto: {ex.Message}");
            }
        }

        public async Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filt = null)
        {
            try
            {

                TEntity entidad = await _context.Set<TEntity>().FirstOrDefaultAsync(filt);
                return entidad;

            }
            catch(Exception ex)
            {
                throw new Exception($"Error al intentar obtener el objeto: {ex.Message}");
            }
        }
    }
}
