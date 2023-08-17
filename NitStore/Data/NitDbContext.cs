using Microsoft.EntityFrameworkCore;
using NitStore.Models.Domain;

namespace NitStore.Data
{
    public class NitDbContext : DbContext
    {

        public NitDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> users { get; set; }
        public DbSet<Role> roles { get; set; }

        public DbSet<Category> categories { get; set; }

        public DbSet<Image> images { get; set; }

        public DbSet<Campaign> campaigns { get; set; }

        public DbSet<CampaignItem> campaignItems { get; set; }

        public DbSet<Feedback> feedbacks { get; set; }

        public DbSet<Order> orders { get; set; }

        public DbSet<Product> products { get;set; }

        public DbSet<OrderDetail> ordersDetail { get; set; }

        public DbSet<ProductImage> productsImage { get; set; }

        public DbSet<Slider> slider { get; set; }

        public DbSet<UserDetail> userDetail { get; set; }

    }
}
