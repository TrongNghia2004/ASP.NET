using luongtrongnghia.Model;
using Microsoft.EntityFrameworkCore;

namespace luongtrongnghia.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor của AppDbContext sẽ nhận các tùy chọn (options) để cấu hình kết nối DB.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Định nghĩa các DbSet cho từng bảng trong cơ sở dữ liệu.
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}
