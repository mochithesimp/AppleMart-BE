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
        IRepository<Product> ProductRepository { get; }
        IRepository<ProductImg> ProductImgRepository { get; }
        IRepository<BlogImage> BlogImageRepository { get; }
        IRepository<Review> ReviewRepository { get; }
        IRepository<ChatRoom> ChatRoomRepository { get; }
        IRepository<ChatMessage> ChatMessageRepository { get; }
        IRepository<ChatParticipant> ChatParticipantRepository { get; }

        void CommitTransaction();
        void RollbackTransaction();
        int SaveChanges();
        void BeginTransaction();
    }
}
