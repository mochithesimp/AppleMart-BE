using iPhoneBE.Data.Data;
using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppleMartDBContext _dbContext;
        private IDbContextTransaction? _transaction = null;

        IRepository<Entities.Attribute> _attributeRepository;
        IRepository<Category> _categoryRepository;
        IRepository<Blog> _blogRepository;
        IRepository<ProductItemAttribute> _productItemAttributeRepository;
        IRepository<Product> _productRepository;
        IRepository<ProductItem> _productItemRepository;
        IRepository<ProductImg> _productImgRepository;
        IRepository<BlogImage> _blogImageRepository;

        public UnitOfWork(
            AppleMartDBContext dbContext,
            IRepository<Category> categoryRepository,
            IRepository<Blog> blogRepository,
            IRepository<Entities.Attribute> attributeRepository,
            IRepository<ProductItemAttribute> productItemAttributeRepository,
            IRepository<ProductItem> productItemRepository,
            IRepository<Product> productRepository,
            IRepository<ProductImg> productImgRepository,
            IRepository<BlogImage> blogImageRepository
        )
        {
            _dbContext = dbContext;
            _categoryRepository = categoryRepository;
            _blogRepository = blogRepository;
            _attributeRepository = attributeRepository;
            _productItemAttributeRepository = productItemAttributeRepository;
            _productItemRepository = productItemRepository;
            _productRepository = productRepository;
            _productImgRepository = productImgRepository;
            _blogImageRepository = blogImageRepository;
        }

        //repository
        public IRepository<Entities.Attribute> AttributeRepository => _attributeRepository;
        public IRepository<Category> CategoryRepository => _categoryRepository;
        public IRepository<Blog> BlogRepository => _blogRepository;
        public IRepository<ProductItemAttribute> ProductItemAttributeRepository => _productItemAttributeRepository;
        public IRepository<ProductItem> ProductItemRepository => _productItemRepository;
        public IRepository<Product> ProductRepository => _productRepository;
        public IRepository<ProductImg> ProductImgRepository => _productImgRepository;
        public IRepository<BlogImage> BlogImageRepository => _blogImageRepository;

        //transaction
        public void BeginTransaction()
        {
            _transaction = _dbContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public void RollbackTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
            }
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
}
