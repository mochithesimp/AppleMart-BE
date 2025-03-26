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
            }
            ;

            if (!context.Users.Any())
            {
                // Thêm người dùng
                var users = new List<(string username, string email, string name, string role)>
                {
                    ("admin", "admin@example.com", "Administrator", RolesHelper.Admin),
                    ("staff", "staff@example.com", "Store Staff", RolesHelper.Staff),
                    ("Vu", "vu@example.com", "Store Shipper", RolesHelper.Shipper),
                    ("Nhan", "nhan@example.com", "Store Shipper", RolesHelper.Shipper),
                    ("customer", "user@example.com", "Regular Customer", RolesHelper.Customer)
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
                    new Category { Name = "iPhone", Description = "Dòng điện thoại thông minh của Apple", IsDeleted = false },
                    new Category { Name = "iPad", Description = "Máy tính bảng của Apple", IsDeleted = false },
                    new Category { Name = "MacBook", Description = "Laptop của Apple", IsDeleted = false },
                    new Category { Name = "iMac", Description = "Máy tính để bàn All-in-One của Apple", IsDeleted = false },
                    new Category { Name = "Apple Watch", Description = "Đồng hồ thông minh của Apple", IsDeleted = false },
                    new Category { Name = "AirPods", Description = "Tai nghe không dây của Apple", IsDeleted = false },
                    new Category { Name = "Accessories", Description = "Phụ kiện chính hãng của Apple", IsDeleted = false }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            if (!context.ShippingMethods.Any())
            {
                var shippingMethod = new ShippingMethod
                {
                    Name = "Standard Shipping",
                    ShippingPrice = 15,
                    IsDeleted = false
                };
                context.ShippingMethods.Add(shippingMethod);
                await context.SaveChangesAsync();
            }

            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product { CategoryID = 1, Name = "iPhone 15", Description = "iPhone 15 Series", IsDeleted = false },
                    new Product { CategoryID = 1, Name = "iPhone 15 Pro", Description = "iPhone 15 Pro Series", IsDeleted = false },
                    new Product { CategoryID = 3, Name = "MacBook Pro", Description = "MacBook Pro 16-inch", IsDeleted = false },
                    new Product { CategoryID = 3, Name = "MacBook Air", Description = "MacBook Air M2", IsDeleted = false },
                    new Product { CategoryID = 6, Name = "AirPods Pro", Description = "Tai nghe AirPods Pro", IsDeleted = false },
                    new Product { CategoryID = 6, Name = "AirPods Max", Description = "Tai nghe trùm đầu AirPods Max", IsDeleted = false },
                    new Product { CategoryID = 2, Name = "iPad Pro", Description = "iPad Pro M2 Series", IsDeleted = false },
                    new Product { CategoryID = 2, Name = "iPad Air", Description = "iPad Air M1 Series", IsDeleted = false },
                    new Product { CategoryID = 5, Name = "Apple Watch Series 9", Description = "Apple Watch Series 9", IsDeleted = false },
                    new Product { CategoryID = 5, Name = "Apple Watch Ultra 2", Description = "Apple Watch Ultra 2", IsDeleted = false },
                    new Product { CategoryID = 4, Name = "iMac 24-inch", Description = "iMac 24-inch M3 Series", IsDeleted = false },
                    new Product { CategoryID = 7, Name = "Apple Pencil", Description = "Apple Pencil Series", IsDeleted = false }
                };
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }

            if (!context.ProductItems.Any())
            {
                var productItems = new List<ProductItem>
                {
                    new ProductItem { ProductID = 1, Name = "iPhone 15 Blue 128GB", Quantity = 1000, Price = 799, IsDeleted = false },
                    new ProductItem { ProductID = 1, Name = "iPhone 15 Red 256GB", Quantity = 1000, Price = 899, IsDeleted = false },
                    new ProductItem { ProductID = 1, Name = "iPhone 15 Black 512GB", Quantity = 1000, Price = 1099, IsDeleted = false },
                    new ProductItem { ProductID = 1, Name = "iPhone 15 Pink 128GB", Quantity = 1000, Price = 799, IsDeleted = false },
                    new ProductItem { ProductID = 1, Name = "iPhone 15 Yellow 256GB", Quantity = 1000, Price = 899, IsDeleted = false },

                    new ProductItem { ProductID = 2, Name = "iPhone 15 Pro Black 256GB", Quantity = 1000, Price = 999, IsDeleted = false },
                    new ProductItem { ProductID = 2, Name = "iPhone 15 Pro Natural 512GB", Quantity = 1000, Price = 1199, IsDeleted = false },
                    new ProductItem { ProductID = 2, Name = "iPhone 15 Pro Blue 1TB", Quantity = 1000, Price = 1399, IsDeleted = false },
                    new ProductItem { ProductID = 2, Name = "iPhone 15 Pro White 256GB", Quantity = 1000, Price = 999, IsDeleted = false },

                    new ProductItem { ProductID = 3, Name = "MacBook Pro Silver 16-inch", Quantity = 1000, Price = 1999, IsDeleted = false },
                    new ProductItem { ProductID = 3, Name = "MacBook Pro Space Gray 16-inch", Quantity = 1000, Price = 2499, IsDeleted = false },
                    new ProductItem { ProductID = 3, Name = "MacBook Pro Black 14-inch", Quantity = 1000, Price = 1599, IsDeleted = false },
                    new ProductItem { ProductID = 3, Name = "MacBook Pro Silver 14-inch", Quantity = 1000, Price = 1599, IsDeleted = false },

                    new ProductItem { ProductID = 4, Name = "MacBook Air Gold M2", Quantity = 1000, Price = 1299, IsDeleted = false },
                    new ProductItem { ProductID = 4, Name = "MacBook Air Silver M2", Quantity = 1000, Price = 1299, IsDeleted = false },
                    new ProductItem { ProductID = 4, Name = "MacBook Air Space Gray M2", Quantity = 1000, Price = 1299, IsDeleted = false },
                    new ProductItem { ProductID = 4, Name = "MacBook Air Midnight M2", Quantity = 1000, Price = 1299, IsDeleted = false },

                    new ProductItem { ProductID = 5, Name = "AirPods Pro White", Quantity = 1000, Price = 249, IsDeleted = false },
                    new ProductItem { ProductID = 5, Name = "AirPods Pro Black", Quantity = 1000, Price = 249, IsDeleted = false },

                    new ProductItem { ProductID = 6, Name = "AirPods Max Silver", Quantity = 1000, Price = 549, IsDeleted = false },
                    new ProductItem { ProductID = 6, Name = "AirPods Max Space Gray", Quantity = 1000, Price = 549, IsDeleted = false },
                    new ProductItem { ProductID = 6, Name = "AirPods Max Sky Blue", Quantity = 1000, Price = 549, IsDeleted = false },
                    new ProductItem { ProductID = 6, Name = "AirPods Max Pink", Quantity = 1000, Price = 549, IsDeleted = false },

                    new ProductItem { ProductID = 7, Name = "iPad Pro 12.9-inch Space Gray 256GB", Quantity = 1000, Price = 1099, IsDeleted = false },
                    new ProductItem { ProductID = 7, Name = "iPad Pro 12.9-inch Silver 512GB", Quantity = 1000, Price = 1299, IsDeleted = false },
                    new ProductItem { ProductID = 7, Name = "iPad Pro 11-inch Space Gray 128GB", Quantity = 1000, Price = 799, IsDeleted = false },
                    new ProductItem { ProductID = 7, Name = "iPad Pro 11-inch Silver 256GB", Quantity = 1000, Price = 899, IsDeleted = false },

                    new ProductItem { ProductID = 8, Name = "iPad Air Blue 64GB", Quantity = 1000, Price = 599, IsDeleted = false },
                    new ProductItem { ProductID = 8, Name = "iPad Air Purple 256GB", Quantity = 1000, Price = 749, IsDeleted = false },
                    new ProductItem { ProductID = 8, Name = "iPad Air Pink 128GB", Quantity = 1000, Price = 649, IsDeleted = false },
                    new ProductItem { ProductID = 8, Name = "iPad Air Starlight 64GB", Quantity = 1000, Price = 599, IsDeleted = false },

                    new ProductItem { ProductID = 9, Name = "Apple Watch Series 9 GPS 41mm Midnight", Quantity = 1000, Price = 399, IsDeleted = false },
                    new ProductItem { ProductID = 9, Name = "Apple Watch Series 9 GPS 45mm Starlight", Quantity = 1000, Price = 429, IsDeleted = false },
                    new ProductItem { ProductID = 9, Name = "Apple Watch Series 9 GPS+Cellular 41mm Pink", Quantity = 1000, Price = 499, IsDeleted = false },
                    new ProductItem { ProductID = 9, Name = "Apple Watch Series 9 GPS+Cellular 45mm Silver", Quantity = 1000, Price = 529, IsDeleted = false },

                    new ProductItem { ProductID = 10, Name = "Apple Watch Ultra 2 Natural Titanium", Quantity = 1000, Price = 799, IsDeleted = false },
                    new ProductItem { ProductID = 10, Name = "Apple Watch Ultra 2 Blue Ocean", Quantity = 1000, Price = 799, IsDeleted = false },

                    new ProductItem { ProductID = 11, Name = "iMac 24-inch Blue 8GB/256GB", Quantity = 1000, Price = 1299, IsDeleted = false },
                    new ProductItem { ProductID = 11, Name = "iMac 24-inch Purple 8GB/512GB", Quantity = 1000, Price = 1499, IsDeleted = false },
                    new ProductItem { ProductID = 11, Name = "iMac 24-inch Pink 16GB/1TB", Quantity = 1000, Price = 1799, IsDeleted = false },
                    new ProductItem { ProductID = 11, Name = "iMac 24-inch Yellow 16GB/2TB", Quantity = 1000, Price = 1999, IsDeleted = false },

                    new ProductItem { ProductID = 12, Name = "Apple Pencil (2nd generation)", Quantity = 1000, Price = 129, IsDeleted = false },
                    new ProductItem { ProductID = 12, Name = "Apple Pencil (USB-C)", Quantity = 1000, Price = 79, IsDeleted = false }
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
                    new Entities.Attribute { AttributeName = "RAM", DataType = "String", CategoryID = 1 },
                    new Entities.Attribute { AttributeName = "Battery", DataType = "String", CategoryID = 1 },
                    new Entities.Attribute { AttributeName = "Screen", DataType = "String", CategoryID = 1 },
                    new Entities.Attribute { AttributeName = "Camera", DataType = "String", CategoryID = 1 },
                    new Entities.Attribute { AttributeName = "Waterproof", DataType = "String", CategoryID = 1 },

                    new Entities.Attribute { AttributeName = "Color", DataType = "String", CategoryID = 2 },
                    new Entities.Attribute { AttributeName = "Storage", DataType = "String", CategoryID = 2 },
                    new Entities.Attribute { AttributeName = "Chip", DataType = "String", CategoryID = 2 },
                    new Entities.Attribute { AttributeName = "RAM", DataType = "String", CategoryID = 2 },
                    new Entities.Attribute { AttributeName = "Screen", DataType = "String", CategoryID = 2 },
                    new Entities.Attribute { AttributeName = "Battery", DataType = "String", CategoryID = 2 },
                    new Entities.Attribute { AttributeName = "Cellular", DataType = "String", CategoryID = 2 },
                    new Entities.Attribute { AttributeName = "Pencil Support", DataType = "String", CategoryID = 2 },

                    new Entities.Attribute { AttributeName = "Color", DataType = "String", CategoryID = 3 },
                    new Entities.Attribute { AttributeName = "Chip", DataType = "String", CategoryID = 3 },
                    new Entities.Attribute { AttributeName = "RAM", DataType = "String", CategoryID = 3 },
                    new Entities.Attribute { AttributeName = "SSD", DataType = "String", CategoryID = 3 },
                    new Entities.Attribute { AttributeName = "Screen", DataType = "String", CategoryID = 3 },
                    new Entities.Attribute { AttributeName = "Battery", DataType = "String", CategoryID = 3 },
                    new Entities.Attribute { AttributeName = "Weight", DataType = "String", CategoryID = 3 },

                    new Entities.Attribute { AttributeName = "Color", DataType = "String", CategoryID = 4 },
                    new Entities.Attribute { AttributeName = "Chip", DataType = "String", CategoryID = 4 },
                    new Entities.Attribute { AttributeName = "RAM", DataType = "String", CategoryID = 4 },
                    new Entities.Attribute { AttributeName = "SSD", DataType = "String", CategoryID = 4 },
                    new Entities.Attribute { AttributeName = "Screen", DataType = "String", CategoryID = 4 },
                    new Entities.Attribute { AttributeName = "Camera", DataType = "String", CategoryID = 4 },
                    new Entities.Attribute { AttributeName = "Speakers", DataType = "String", CategoryID = 4 },

                    new Entities.Attribute { AttributeName = "Color", DataType = "String", CategoryID = 5 },
                    new Entities.Attribute { AttributeName = "Size", DataType = "String", CategoryID = 5 },
                    new Entities.Attribute { AttributeName = "Material", DataType = "String", CategoryID = 5 },
                    new Entities.Attribute { AttributeName = "Battery", DataType = "String", CategoryID = 5 },
                    new Entities.Attribute { AttributeName = "Waterproof", DataType = "String", CategoryID = 5 },
                    new Entities.Attribute { AttributeName = "Cellular", DataType = "String", CategoryID = 5 },
                    new Entities.Attribute { AttributeName = "GPS", DataType = "String", CategoryID = 5 },

                    new Entities.Attribute { AttributeName = "Color", DataType = "String", CategoryID = 6 },
                    new Entities.Attribute { AttributeName = "Waterproof", DataType = "String", CategoryID = 6 },
                    new Entities.Attribute { AttributeName = "Battery", DataType = "String", CategoryID = 6 },
                    new Entities.Attribute { AttributeName = "Noise Cancellation", DataType = "String", CategoryID = 6 },
                    new Entities.Attribute { AttributeName = "Connectivity", DataType = "String", CategoryID = 6 },
                    new Entities.Attribute { AttributeName = "Case Type", DataType = "String", CategoryID = 6 },

                    new Entities.Attribute { AttributeName = "Color", DataType = "String", CategoryID = 7 },
                    new Entities.Attribute { AttributeName = "Compatibility", DataType = "String", CategoryID = 7 },
                    new Entities.Attribute { AttributeName = "Connectivity", DataType = "String", CategoryID = 7 },
                    new Entities.Attribute { AttributeName = "Battery", DataType = "String", CategoryID = 7 },
                    new Entities.Attribute { AttributeName = "Features", DataType = "String", CategoryID = 7 }
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
                    new ProductItemAttribute { ProductItemID = 1, AttributeID = 3, Value = "A16 Bionic" },
                    new ProductItemAttribute { ProductItemID = 1, AttributeID = 4, Value = "6GB" },
                    new ProductItemAttribute { ProductItemID = 1, AttributeID = 5, Value = "Up to 20 hours" },
                    new ProductItemAttribute { ProductItemID = 1, AttributeID = 6, Value = "6.1-inch Super Retina XDR" },
                    new ProductItemAttribute { ProductItemID = 1, AttributeID = 7, Value = "48MP Main, 12MP Ultra Wide, 12MP Telephoto" },
                    new ProductItemAttribute { ProductItemID = 1, AttributeID = 8, Value = "IP68" },

                    new ProductItemAttribute { ProductItemID = 21, AttributeID = 9, Value = "Space Gray" },
                    new ProductItemAttribute { ProductItemID = 21, AttributeID = 10, Value = "256GB" },
                    new ProductItemAttribute { ProductItemID = 21, AttributeID = 11, Value = "M2" },
                    new ProductItemAttribute { ProductItemID = 21, AttributeID = 12, Value = "8GB" },
                    new ProductItemAttribute { ProductItemID = 21, AttributeID = 13, Value = "12.9-inch Liquid Retina XDR" },
                    new ProductItemAttribute { ProductItemID = 21, AttributeID = 14, Value = "Up to 10 hours" },
                    new ProductItemAttribute { ProductItemID = 21, AttributeID = 15, Value = "Wi-Fi + Cellular" },
                    new ProductItemAttribute { ProductItemID = 21, AttributeID = 16, Value = "Apple Pencil (2nd generation)" },

                    new ProductItemAttribute { ProductItemID = 25, AttributeID = 17, Value = "Midnight" },
                    new ProductItemAttribute { ProductItemID = 25, AttributeID = 18, Value = "41mm" },
                    new ProductItemAttribute { ProductItemID = 25, AttributeID = 19, Value = "Aluminum" },
                    new ProductItemAttribute { ProductItemID = 25, AttributeID = 20, Value = "Up to 18 hours" },
                    new ProductItemAttribute { ProductItemID = 25, AttributeID = 21, Value = "Water resistant 50m" },
                    new ProductItemAttribute { ProductItemID = 25, AttributeID = 22, Value = "GPS only" },
                    new ProductItemAttribute { ProductItemID = 25, AttributeID = 23, Value = "GPS" },

                    new ProductItemAttribute { ProductItemID = 29, AttributeID = 24, Value = "Blue" },
                    new ProductItemAttribute { ProductItemID = 29, AttributeID = 25, Value = "M3" },
                    new ProductItemAttribute { ProductItemID = 29, AttributeID = 26, Value = "8GB" },
                    new ProductItemAttribute { ProductItemID = 29, AttributeID = 27, Value = "256GB" },
                    new ProductItemAttribute { ProductItemID = 29, AttributeID = 28, Value = "24-inch 4.5K Retina" },
                    new ProductItemAttribute { ProductItemID = 29, AttributeID = 29, Value = "1080p FaceTime HD" },
                    new ProductItemAttribute { ProductItemID = 29, AttributeID = 30, Value = "Six-speaker sound system" },

                    new ProductItemAttribute { ProductItemID = 33, AttributeID = 31, Value = "White" },
                    new ProductItemAttribute { ProductItemID = 33, AttributeID = 32, Value = "iPad Pro 12.9-inch (3rd and 4th generation), iPad Pro 11-inch (1st and 2nd generation), iPad Air (4th generation), iPad mini (6th generation)" },
                    new ProductItemAttribute { ProductItemID = 33, AttributeID = 33, Value = "Magnetic" },
                    new ProductItemAttribute { ProductItemID = 33, AttributeID = 34, Value = "Up to 12 hours" },
                    new ProductItemAttribute { ProductItemID = 33, AttributeID = 35, Value = "Pressure sensitivity, tilt support, wireless charging" }
                };
                context.ProductItemAttributes.AddRange(productItemAttributes);
                await context.SaveChangesAsync();
            }

            if (!context.ProductImgs.Any())
            {
                var productImgs = new List<ProductImg>
                {
                    new ProductImg { ProductItemID = 1, ImageUrl = "https://r4k.com.au/wp-content/uploads/2023/11/no-upfront-cost-apple-iphone-15-128gb-blue.jpg", IsDeleted = false },
                    new ProductImg { ProductItemID = 2, ImageUrl = "https://th.bing.com/th/id/OIP.NssiQUFE8Uvv__sa99doegHaHa?rs=1&pid=ImgDetMain", IsDeleted = false },
                    new ProductImg { ProductItemID = 3, ImageUrl = "https://th.bing.com/th/id/OIP.4buvegdxey0F75FrSJwM5AHaHa?rs=1&pid=ImgDetMain", IsDeleted = false },
                    new ProductImg { ProductItemID = 4, ImageUrl = "https://th.bing.com/th/id/OIP.6oMBJQOaRrV2qw-g1vNbFgHaJf?rs=1&pid=ImgDetMain", IsDeleted = false },
                    new ProductImg { ProductItemID = 5, ImageUrl = "https://media.currys.biz/i/currysprod/M10254987_yellow?$l-large$&fmt=auto", IsDeleted = false },

                    new ProductImg { ProductItemID = 6, ImageUrl = "https://th.bing.com/th/id/R.e1bbaf7a07717b2d05ef916b489c0ce4?rik=C9%2bd0o1n%2bxh1Xg&riu=http%3a%2f%2fpowermaccenter.com%2fcdn%2fshop%2ffiles%2fiPhone_15_Pro_Max_Black_Titanium_PDP_Image_Position-1__en-US_f79700b1-361f-411b-abc0-0f8706d12ee1.jpg%3fv%3d1695860765&ehk=1xAse4OipR%2fn9jukP%2bE3imNyZaWtaz3waALDuB%2fPzmI%3d&risl=&pid=ImgRaw&r=0", IsDeleted = false },
                    new ProductImg { ProductItemID = 7, ImageUrl = "https://th.bing.com/th/id/OIP.fReV02NTtSbL4phRCp02LwHaEK?rs=1&pid=ImgDetMain", IsDeleted = false },
                    new ProductImg { ProductItemID = 8, ImageUrl = "https://th.bing.com/th/id/OIP.mq0_wnnHDBxFTT157Vhk_gHaHa?w=500&h=500&rs=1&pid=ImgDetMain", IsDeleted = false },
                    new ProductImg { ProductItemID = 9, ImageUrl = "https://media-ik.croma.com/prod/https://media.croma.com/image/upload/v1694673405/Croma%20Assets/Communication/Mobiles/Images/300757_0_xr6ypa.png", IsDeleted = false },

                    new ProductImg { ProductItemID = 10, ImageUrl = "https://th.bing.com/th/id/R.51fe5e728ee1e17bb7f76226163c4ca5?rik=tGTmoXAgNzjtGA&pid=ImgRaw&r=0", IsDeleted = false },
                    new ProductImg { ProductItemID = 11, ImageUrl = "https://th.bing.com/th/id/OIP.XKGHt1wg3NvyQsVg_CVV9AHaHa?rs=1&pid=ImgDetMain", IsDeleted = false },
                    new ProductImg { ProductItemID = 12, ImageUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/mbp14-spaceblack-cto-hero-202410?wid=1200&hei=630&fmt=jpeg&qlt=95&.v=1731525368099", IsDeleted = false },
                    new ProductImg { ProductItemID = 13, ImageUrl = "https://store.storeimages.cdn-apple.com/4668/as-images.apple.com/is/mbp14-silver-select-202110_GEO_AE?wid=1200&hei=630&fmt=jpeg&qlt=95&.v=1647362605401", IsDeleted = false },

                    new ProductImg { ProductItemID = 14, ImageUrl = "https://th.bing.com/th/id/R.f6295f78e944a59dbb2d22d804f0ee85?rik=DmD3IUTqsOs4KQ&pid=ImgRaw&r=0", IsDeleted = false },
                    new ProductImg { ProductItemID = 15, ImageUrl = "https://shopdunk.com/images/uploaded/macbook-air-m2-silver-1.jpg", IsDeleted = false },
                    new ProductImg { ProductItemID = 16, ImageUrl = "https://th.bing.com/th/id/OIP.GRNyMIrtkhT7qJKCi4FhRgHaFs?rs=1&pid=ImgDetMain", IsDeleted = false },
                    new ProductImg { ProductItemID = 17, ImageUrl = "https://th.bing.com/th/id/R.099d1a30d9dd9cfa20fc36ecbdf68081?rik=MELRDKiuLfNyNA&pid=ImgRaw&r=0", IsDeleted = false },

                    new ProductImg { ProductItemID = 18, ImageUrl = "https://th.bing.com/th/id/OIP.IskpZw_vU_MnlqLOBIgqmAHaGk?rs=1&pid=ImgDetMain", IsDeleted = false },
                    new ProductImg { ProductItemID = 19, ImageUrl = "https://th.bing.com/th/id/OIP.euZRrYpbzbhv56YzsiLFUgHaHa?rs=1&pid=ImgDetMain", IsDeleted = false },

                    new ProductImg { ProductItemID = 20, ImageUrl = "https://www.bhphotovideo.com/images/images2500x2500/apple_mgyj3am_a_airpods_max_silver_1610234.jpg", IsDeleted = false },
                    new ProductImg { ProductItemID = 21, ImageUrl = "https://pisces.bbystatic.com/image2/BestBuy_US/images/products/6373/6373460cv11d.jpg", IsDeleted = false },
                    new ProductImg { ProductItemID = 22, ImageUrl = "https://th.bing.com/th/id/OIP.oxfxs9JZRq_xHmJL6XtdPQHaHa?rs=1&pid=ImgDetMain", IsDeleted = false },
                    new ProductImg { ProductItemID = 23, ImageUrl = "https://m.media-amazon.com/images/I/51f31ECzanL._AC_SL1500_.jpg", IsDeleted = false },

                    new ProductImg { ProductItemID = 24, ImageUrl = "https://th.bing.com/th/id/R.570d468ba62c1ae56b0731e863feeeb1?rik=uLYujkAtitl%2fTA&pid=ImgRaw&r=0", IsDeleted = false },
                    new ProductImg { ProductItemID = 25, ImageUrl = "https://th.bing.com/th/id/OIP.ljex460PB5_j0QY6VlYBsgHaHa?rs=1&pid=ImgDetMain", IsDeleted = false },
                    new ProductImg { ProductItemID = 26, ImageUrl = "https://pisces.bbystatic.com/image2/BestBuy_US/images/products/3755/3755015_sd.jpg", IsDeleted = false },
                    new ProductImg { ProductItemID = 27, ImageUrl = "https://store.storeimages.cdn-apple.com/4668/as-images.apple.com/is/refurb-ipad-pro-11-wifi-silver-2021?wid=1144&hei=1144&fmt=jpeg&qlt=90&.v=1674663704665", IsDeleted = false },

                    new ProductImg { ProductItemID = 28, ImageUrl = "https://th.bing.com/th/id/R.425e9731ccef254aff8ab62293857bea?rik=sFKjhzUO1%2fpwlA&pid=ImgRaw&r=0", IsDeleted = false },
                    new ProductImg { ProductItemID = 29, ImageUrl = "https://th.bing.com/th/id/OIP.hDw09O_U8XkZbKvZTxEy-gHaHa?rs=1&pid=ImgDetMain", IsDeleted = false },
                    new ProductImg { ProductItemID = 30, ImageUrl = "https://th.bing.com/th/id/OIP.Ibkx-voEOWUdJEFlfPs2GgHaHa?rs=1&pid=ImgDetMain", IsDeleted = false },
                    new ProductImg { ProductItemID = 31, ImageUrl = "https://expertlaois.ie/wp-content/uploads/2022/10/APPLE-IPAD-AIR-STARLIGHT.jpg", IsDeleted = false },

                    new ProductImg { ProductItemID = 32, ImageUrl = "https://th.bing.com/th/id/OIP.dQwi9KnaFg47d8sUFE79VAHaIs?rs=1&pid=ImgDetMain", IsDeleted = false },
                    new ProductImg { ProductItemID = 33, ImageUrl = "https://th.bing.com/th/id/OIP.pW6bSjbBmarVRIGC3eODOQHaHa?rs=1&pid=ImgDetMain", IsDeleted = false },
                    new ProductImg { ProductItemID = 34, ImageUrl = "https://store.storeimages.cdn-apple.com/4668/as-images.apple.com/is/41-cell-alum-pink-sport-band-light-pink-s9?wid=1200&hei=630&fmt=jpeg&qlt=95&.v=1693282238597", IsDeleted = false },
                    new ProductImg { ProductItemID = 35, ImageUrl = "https://th.bing.com/th/id/OIP.xNx2-aJLplkBRWxY4WJ7NAHaHa?rs=1&pid=ImgDetMain", IsDeleted = false },

                    new ProductImg { ProductItemID = 36, ImageUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/49-cell-titanium-natural-milanese-loop-natural-ultra?wid=1200&hei=630&fmt=jpeg&qlt=95&.v=1724552757754", IsDeleted = false },
                    new ProductImg { ProductItemID = 37, ImageUrl = "https://th.bing.com/th/id/OIP.ls3XlKw1K6R7MlO67Eeu0AAAAA?rs=1&pid=ImgDetMain", IsDeleted = false },

                    new ProductImg { ProductItemID = 38, ImageUrl = "https://microless.com/cdn/products/2ec449a4312827988cff9e9dd37ebc79-hi.jpg", IsDeleted = false },
                    new ProductImg { ProductItemID = 39, ImageUrl = "https://th.bing.com/th/id/OIP.uTJZD5v4mbYWAMZCq0lHNQHaHa?rs=1&pid=ImgDetMain", IsDeleted = false },
                    new ProductImg { ProductItemID = 40, ImageUrl = "https://macfinder.co.uk/wp-content/uploads/2023/01/img-iMac-24-Inch-12092.jpg", IsDeleted = false },
                    new ProductImg { ProductItemID = 41, ImageUrl = "https://th.bing.com/th/id/OIP._EvBuOgE7p0Bv7UVRbvBggHaHa?rs=1&pid=ImgDetMain", IsDeleted = false },

                    new ProductImg { ProductItemID = 42, ImageUrl = "https://th.bing.com/th/id/OIP.rQm6gK-WNKOE2joEAZlwhAHaHa?rs=1&pid=ImgDetMain", IsDeleted = false },
                    new ProductImg { ProductItemID = 43, ImageUrl = "https://i.pcmag.com/imagery/articles/02rRlTJAGmKqIz3kedm6PAf-2.fit_lim.v1697549089.jpg", IsDeleted = false }
                };
                context.ProductImgs.AddRange(productImgs);
                await context.SaveChangesAsync();
            }

        }
    }
}
