using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Entities.Attribute> AttributeRepository { get; }
        IRepository<Category> CategoryRepository { get; }
        IRepository<Blog> BlogRepository { get; }
        IRepository<ProductItemAttribute> ProductItemAttributeRepository { get; }
        IRepository<ProductItem> ProductItemRepository { get; }


        void CommitTransaction();
        void RollbackTransaction();
        int SaveChanges();
        void BeginTransaction();
    }
}
