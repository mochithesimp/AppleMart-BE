using iPhoneBE.Data.Data;
using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Interfaces;
using iPhoneBE.Data.Model;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace iPhoneBE.Data
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private readonly AppleMartDBContext _dbContext;
        private IDbContextTransaction? _transaction = null;

        // 🔹 Thêm `private readonly` để đảm bảo các repository không bị thay đổi sau khi khởi tạo
        private readonly IRepository<Entities.Attribute> _attributeRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Blog> _blogRepository;
        private readonly IRepository<ProductItemAttribute> _productItemAttributeRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductItem> _productItemRepository;
        private readonly IRepository<ProductImg> _productImgRepository;
        private readonly IRepository<BlogImage> _blogImageRepository;
        private readonly IRepository<Review> _reviewRepository;
        private readonly IRepository<ChatRoom> _chatRoomRepository;
        private readonly IRepository<ChatMessage> _chatMessageRepository;
        private readonly IRepository<ChatParticipant> _chatParticipantRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderDetail> _orderDetailRepository;
        private readonly IRepository<User> _userRepository;
        public UnitOfWork(
            AppleMartDBContext dbContext,
            IRepository<User> userRepository,
            IRepository<Category> categoryRepository,
            IRepository<Blog> blogRepository,
            IRepository<Entities.Attribute> attributeRepository,
            IRepository<ProductItemAttribute> productItemAttributeRepository,
            IRepository<ProductItem> productItemRepository,
            IRepository<Product> productRepository,
            IRepository<ProductImg> productImgRepository,
            IRepository<BlogImage> blogImageRepository,
            IRepository<Review> reviewRepository,
            IRepository<ChatRoom> chatRoomRepository,
            IRepository<ChatMessage> chatMessageRepository,
            IRepository<ChatParticipant> chatParticipantRepository,
            IRepository<Order> orderRepository,
            IRepository<OrderDetail> orderDetailRepository
        )
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _blogRepository = blogRepository;
            _attributeRepository = attributeRepository;
            _productItemAttributeRepository = productItemAttributeRepository;
            _productItemRepository = productItemRepository;
            _productRepository = productRepository;
            _productImgRepository = productImgRepository;
            _blogImageRepository = blogImageRepository;
            _reviewRepository = reviewRepository;
            _chatRoomRepository = chatRoomRepository;
            _chatMessageRepository = chatMessageRepository;
            _chatParticipantRepository = chatParticipantRepository;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        // 🔹 Repository getter
        public IRepository<Entities.Attribute> AttributeRepository => _attributeRepository;
        public IRepository<Category> CategoryRepository => _categoryRepository;
        public IRepository<Blog> BlogRepository => _blogRepository;
        public IRepository<ProductItemAttribute> ProductItemAttributeRepository => _productItemAttributeRepository;
        public IRepository<ProductItem> ProductItemRepository => _productItemRepository;
        public IRepository<Product> ProductRepository => _productRepository;
        public IRepository<ProductImg> ProductImgRepository => _productImgRepository;
        public IRepository<BlogImage> BlogImageRepository => _blogImageRepository;
        public IRepository<Review> ReviewRepository => _reviewRepository;
        public IRepository<ChatRoom> ChatRoomRepository => _chatRoomRepository;
        public IRepository<ChatMessage> ChatMessageRepository => _chatMessageRepository;
        public IRepository<ChatParticipant> ChatParticipantRepository => _chatParticipantRepository;
        public IRepository<Order> OrderRepository => _orderRepository;
        public IRepository<OrderDetail> OrderDetailRepository => _orderDetailRepository;

        public IRepository<User> UserRepository => _userRepository;

        // 🔹 Transaction - Dùng async để tránh block luồng
        public async Task BeginTransactionAsync()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        // 🔹 Lưu thay đổi vào DB - Thêm async để dùng trong môi trường bất đồng bộ
        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
