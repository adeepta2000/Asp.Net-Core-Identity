using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserCertificateCreationSystem.Common;
using UserCertificateCreationSystem.Model.DBEntity;
using UserCertificateCreationSystem.Repository;

namespace UserCertificateCreationSystem.Services
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public BaseService(IGenericRepository<T> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<T> GetById(int id)
        {
            return await _repository.GetById(id);
        }
        public async Task<OperationResult> Add(T entity)
        {
            await _repository.Add(entity);
            OperationResult result = await _unitOfWork.SaveAsync();
            result.Result = entity;
            return result;
        }

        public async Task<OperationResult> Update(T entityToUpdate)
        {
            return await _repository.Update(entityToUpdate);
        }

        public OperationResult Delete(int id)
        {
            _repository.Delete(id);
            return _unitOfWork.Save();
        }

    }

    public interface IBaseService<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task<OperationResult> Add(T entity);
        Task<OperationResult> Update(T entityToUpdate);
        OperationResult Delete(int id);
    }
}
