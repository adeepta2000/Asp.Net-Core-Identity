using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UserCertificateCreationSystem.Common;
using UserCertificateCreationSystem.Model;

namespace UserCertificateCreationSystem.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDBContext _context;
        internal DbSet<TEntity> DbSet;

        public GenericRepository(ApplicationDBContext context)
        {
            _context = context;
            DbSet = context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await DbSet.ToListAsync();
        }

        public async Task<TEntity> GetById(int id)
        {
            return await DbSet.FindAsync(id);
        }
        public async Task<bool> Add(TEntity entity)
        {
            try
            {
                await DbSet.AddAsync(entity);
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        public async Task<OperationResult> Update(TEntity entityToUpdate)
        {
            try
            {
                DbSet.Update(entityToUpdate);
                await _context.SaveChangesAsync();
                return new OperationResult(true, entityToUpdate, "Data Update Success.");
            }
            catch (Exception ex)
            {
                return new OperationResult(false, entityToUpdate, ex.Message);
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var entity = await DbSet.FindAsync(id);

                if (entity == null)
                    return false;

                DbSet.Remove(entity);
                
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }
    }

    public interface IGenericRepository<TEntity> where TEntity:class
    {
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> GetById(int id);
        Task<bool> Add(TEntity entity);
        Task<OperationResult> Update(TEntity entity);
        Task<bool> Delete(int id);
    }
}
