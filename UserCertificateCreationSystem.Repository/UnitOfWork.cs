using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserCertificateCreationSystem.Common;
using UserCertificateCreationSystem.Model;

namespace UserCertificateCreationSystem.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _context;

        public UnitOfWork(ApplicationDBContext context)
        {
            _context = context;
        }

        public OperationResult Save()
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.SaveChanges();
                    dbContextTransaction.Commit();

                    return new OperationResult(true, null, "Data Save Success.");
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return new OperationResult(false, null, ex.Message);
                }
            }
        }

        public async Task<OperationResult> SaveAsync()
        {
            using (var dbContextTransaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.SaveChangesAsync();
                    await dbContextTransaction.CommitAsync();

                    return new OperationResult(true, null, "Data Save Success.");
                }
                catch (Exception ex)
                {
                    await dbContextTransaction.RollbackAsync();
                    return new OperationResult(false, null, ex.Message);
                }
            }
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }

    public interface IUnitOfWork : IDisposable
    {
        OperationResult Save();
        Task<OperationResult> SaveAsync();

    }
}
