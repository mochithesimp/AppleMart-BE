using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Model;

namespace iPhoneBE.Data.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Entities.Attribute> AttributeRepository { get; }
        IRepository<BlogImage> BlogImageRepository { get; }
        IRepository<Blog> BlogRepository { get; }
        IRepository<Category> CategoryRepository { get; }
        IRepository<ChatMessage> ChatMessageRepository { get; }
        IRepository<ChatParticipant> ChatParticipantRepository { get; }
        IRepository<ChatRoom> ChatRoomRepository { get; }
        IRepository<OrderDetail> OrderDetailRepository { get; }
        IRepository<Order> OrderRepository { get; }
        IRepository<ProductImg> ProductImgRepository { get; }
        IRepository<ProductItemAttribute> ProductItemAttributeRepository { get; }
        IRepository<ProductItem> ProductItemRepository { get; }
        IRepository<Product> ProductRepository { get; }
        IRepository<Review> ReviewRepository { get; }
        IRepository<User> UserRepository { get; }

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        void Dispose();
        Task RollbackTransactionAsync();
        Task<int> SaveChangesAsync();
    }
}