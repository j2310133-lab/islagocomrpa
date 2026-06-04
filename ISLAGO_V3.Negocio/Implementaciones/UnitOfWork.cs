using ISLAGO_V3.Datos;
using ISLAGO_V3.Negocio.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISLAGO_V3.Negocio.Implementaciones
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DBContextISLAGO _context;

        public UnitOfWork(DBContextISLAGO context)
        {
            _context = context;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
