using iPhoneBE.Data.Data;
using iPhoneBE.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public Task<TEntity> AddAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task AddRangeAsync(List<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, params Expression<Func<TEntity, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity?> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetSingleByConditionAsynce(Expression<Func<TEntity, bool>> predicate = null, params Expression<Func<TEntity, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HardRemove(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HardRemoveRange(List<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SoftDelete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SoftDeleteRange(List<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRange(List<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task CommitAsync()
        {
            throw new NotImplementedException();
        }
    }
}
