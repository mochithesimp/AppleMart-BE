using iPhoneBE.Data.Entities;
using iPhoneBE.Data.Helper;
using iPhoneBE.Data.Model;
using Microsoft.AspNetCore.Identity;

namespace iPhoneBE.Data.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(AppleMartDBContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Tạo các vai trò (Admin, Staff, Customer, Shipper)
            if (!context.Roles.Any())
            {
                var roles = new List<string> { "Admin", "Staff", "Customer", "Shipper" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
                await context.SaveChangesAsync();
            };

            if (!context.Users.Any())
            {
                // Thêm người dùng
                var users = new List<(string username, string email, string name, string role)>
                {
                    ("admin", "admin@example.com", "Administrator", RolesHelper.Admin),
                    ("staff", "staff@example.com", "Store Staff", RolesHelper.Staff),
                    ("Vu", "vu@example.com", "Store Shipper", RolesHelper.Shipper),
                    ("Nhan", "nhan@example.com", "Store Shipper", RolesHelper.Shipper),
                    ("customer", "customer@example.com", "Regular Customer", RolesHelper.Customer)
                };

                foreach (var (username, email, name, role) in users)
                {
                    if (await userManager.FindByEmailAsync(email) == null)
                    {
                        var user = new User { UserName = username, Email = email, Name = name, EmailConfirmed = true };
                        await userManager.CreateAsync(user, "123");
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
                await context.SaveChangesAsync();
            }

            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "iPhone", Description = "Dòng điện thoại thông minh của Apple", IsActive = true },
                    new Category { Name = "iPad", Description = "Máy tính bảng của Apple", IsActive = true },
                    new Category { Name = "MacBook", Description = "Laptop của Apple", IsActive = true },
                    new Category { Name = "iMac", Description = "Máy tính để bàn All-in-One của Apple", IsActive = true },
                    new Category { Name = "Apple Watch", Description = "Đồng hồ thông minh của Apple", IsActive = true },
                    new Category { Name = "AirPods", Description = "Tai nghe không dây của Apple", IsActive = true },
                    new Category { Name = "Accessories", Description = "Phụ kiện chính hãng của Apple", IsActive = true }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product { CategoryID = 1, Name = "iPhone 15", Description = "iPhone 15 Series" },
                    new Product { CategoryID = 1, Name = "iPhone 15 Pro", Description = "iPhone 15 Pro Series" },
                    new Product { CategoryID = 3, Name = "MacBook Pro", Description = "MacBook Pro 16-inch" },
                    new Product { CategoryID = 3, Name = "MacBook Air", Description = "MacBook Air M2" },
                    new Product { CategoryID = 6, Name = "AirPods Pro", Description = "Tai nghe AirPods Pro" },
                    new Product { CategoryID = 6, Name = "AirPods Max", Description = "Tai nghe trùm đầu AirPods Max" }
                };
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }

            if (!context.ProductItems.Any())
            {
                var productItems = new List<ProductItem>
                {
                    new ProductItem { ProductID = 1, Name = "iPhone 15 Blue 128GB", Quantity = 1000, Price = 799 },
                    new ProductItem { ProductID = 1, Name = "iPhone 15 Red 256GB", Quantity = 1000, Price = 899 },
                    new ProductItem { ProductID = 2, Name = "iPhone 15 Pro Black 256GB", Quantity = 1000, Price = 999 },
                    new ProductItem { ProductID = 3, Name = "MacBook Pro Silver 16-inch", Quantity = 1000, Price = 1999 },
                    new ProductItem { ProductID = 3, Name = "MacBook Pro Space Gray 16-inch", Quantity = 1000, Price = 2499 },
                    new ProductItem { ProductID = 4, Name = "MacBook Air Gold M2", Quantity = 1000, Price = 1299 },
                    new ProductItem { ProductID = 5, Name = "AirPods Pro White", Quantity = 1000, Price = 249 },
                    new ProductItem { ProductID = 5, Name = "AirPods Pro Black", Quantity = 1000, Price = 249 },
                    new ProductItem { ProductID = 6, Name = "AirPods Max Silver", Quantity = 1000, Price = 549 },
                    new ProductItem { ProductID = 6, Name = "AirPods Max Space Gray", Quantity = 1000, Price = 549 }
                };
                context.ProductItems.AddRange(productItems);
                await context.SaveChangesAsync();
            }

            if (!context.Attributes.Any())
            {
                var attributes = new List<Entities.Attribute>
                {
                    new Entities.Attribute { AttributeName = "Color", DataType = "String", CategoryID = 1 },
                    new Entities.Attribute { AttributeName = "Storage", DataType = "String", CategoryID = 1 },
                    new Entities.Attribute { AttributeName = "Chip", DataType = "String", CategoryID = 1 },
                    new Entities.Attribute { AttributeName = "Screen", DataType = "String", CategoryID = 3 },
                    new Entities.Attribute { AttributeName = "RAM", DataType = "String", CategoryID = 3 },
                    new Entities.Attribute { AttributeName = "Waterproof", DataType = "String", CategoryID = 6 }
                };
                context.Attributes.AddRange(attributes);
                await context.SaveChangesAsync();
            }

            if (!context.ProductItemAttributes.Any())
            {
                var productItemAttributes = new List<ProductItemAttribute>
                {
                    new ProductItemAttribute { ProductItemID = 1, AttributeID = 1, Value = "Blue" },
                    new ProductItemAttribute { ProductItemID = 1, AttributeID = 2, Value = "128GB" },
                    new ProductItemAttribute { ProductItemID = 2, AttributeID = 1, Value = "Red" },
                    new ProductItemAttribute { ProductItemID = 2, AttributeID = 2, Value = "256GB" },
                    new ProductItemAttribute { ProductItemID = 3, AttributeID = 1, Value = "Black" },
                    new ProductItemAttribute { ProductItemID = 3, AttributeID = 2, Value = "256GB" },
                    new ProductItemAttribute { ProductItemID = 4, AttributeID = 1, Value = "Silver" },
                    new ProductItemAttribute { ProductItemID = 4, AttributeID = 4, Value = "16-inch" },
                    new ProductItemAttribute { ProductItemID = 4, AttributeID = 5, Value = "16GB" },
                    new ProductItemAttribute { ProductItemID = 5, AttributeID = 1, Value = "Gold" },
                    new ProductItemAttribute { ProductItemID = 5, AttributeID = 4, Value = "13-inch" },
                    new ProductItemAttribute { ProductItemID = 6, AttributeID = 1, Value = "White" },
                    new ProductItemAttribute { ProductItemID = 6, AttributeID = 6, Value = "IPX4" }
                };
                context.ProductItemAttributes.AddRange(productItemAttributes);
                await context.SaveChangesAsync();
            }

        }
    }
}
