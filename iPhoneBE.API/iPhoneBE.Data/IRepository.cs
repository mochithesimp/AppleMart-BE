using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task Add(TEntity entity);
        Task Add(IEnumerable<TEntity> entities);
        TEntity GetById(object id);
        IQueryable<TEntity> GetAll();
        Task Update(TEntity entity);
        Task Update(IEnumerable<TEntity> entities);
        void Remove(int id);
        void Remove(TEntity entity);
        void Remove(params TEntity[] entities);
        void Remove(IEnumerable<TEntity> entities);





        Task CommitAsync();
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity?> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> GetSingleByConditionAsynce(Expression<Func<TEntity, bool>> predicate = null, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> AddAsync(TEntity entity);
        Task AddRangeAsync(List<TEntity> entities);
        Task<bool> Update1(TEntity entity);
        Task<bool> UpdateRange(List<TEntity> entities);
        Task<bool> SoftDelete(TEntity entity);
        Task<bool> SoftDeleteRange(List<TEntity> entities);
        Task<bool> HardRemove(Expression<Func<TEntity, bool>> predicate);
        Task<bool> HardRemoveRange(List<TEntity> entities);

    }
}
