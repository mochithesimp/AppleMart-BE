using iPhoneBE.Data.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPhoneBE.Data.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(AppleMartDBContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (context.Users.Any()) return; // Nếu đã có user thì không làm gì cả

            // Tạo các vai trò (Admin, Staff, Customer)
            var roles = new List<string> { "Admin", "Staff", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Thêm Admin
            var admin = new User
            {
                UserName = "admin",
                Email = "admin@example.com",
                Name = "Administrator",
                EmailConfirmed = true
            };
            if (await userManager.FindByEmailAsync(admin.Email) == null)
            {
                await userManager.CreateAsync(admin, "123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Thêm Staff
            var staff = new User
            {
                UserName = "staff",
                Email = "staff@example.com",
                Name = "Store Staff",
                EmailConfirmed = true
            };
            if (await userManager.FindByEmailAsync(staff.Email) == null)
            {
                await userManager.CreateAsync(staff, "123");
                await userManager.AddToRoleAsync(staff, "Staff");
            }

            // Thêm Customer
            var customer = new User
            {
                UserName = "customer",
                Email = "customer@example.com",
                Name = "Regular Customer",
                EmailConfirmed = true
            };
            if (await userManager.FindByEmailAsync(customer.Email) == null)
            {
                await userManager.CreateAsync(customer, "123");
                await userManager.AddToRoleAsync(customer, "Customer");
            }

            await context.SaveChangesAsync();
        }
    }
}
