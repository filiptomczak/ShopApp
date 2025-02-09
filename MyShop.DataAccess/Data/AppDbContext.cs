using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyShop.Models;

namespace MyShop.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
             
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts{ get; set; }
        public DbSet<OrderHeader> OrderHeaders{ get; set; }
        public DbSet<OrderDetail> OrderDetails{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //required beacuse of identytyDbContext
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Fantasy", DisplayOrder = 1 },
                new Category { Id = 2, Name = "History", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Action", DisplayOrder = 3 }
                );
            
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Lord Of The Rings",
                    Author = "J.R.R. Tolkien",
                    Description = "Lorem Ipsum",
                    ISBN = "1111PPPP",
                    ListPrice = 30,
                    Price = 27,
                    Price50 = 25,
                    Price100 = 22,
                    CategoryId=1,
                    ImageUrl="",
                },
                new Product
                {
                    Id = 2,
                    Title = "Harry Potter",
                    Author = "J.K. Rowling",
                    Description = "Lorem Ipsum",
                    ISBN = "2P2P2P2P",
                    ListPrice = 20,
                    Price = 17,
                    Price50 = 15,
                    Price100 = 12,
                    CategoryId = 1,
                    ImageUrl = "",

                },
                new Product
                {
                    Id = 3,
                    Title = "The Grapes Of Wrath",
                    Author = "John Steinbeck",
                    Description = "Lorem Ipsum",
                    ISBN = "11PP11PP",
                    ListPrice = 33,
                    Price = 30,
                    Price50 = 29,
                    Price100 = 25,
                    CategoryId = 1,
                    ImageUrl = "",

                }
            );

            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    Id = 1,
                    Name = "Company",
                    Street = "Warszawska 349",
                    State = "Wielkopolska",
                    City = "Bolewice",
                    PostalCode = "62-300",
                    PhoneNumber = "666666666",
                }
            );
        }
    }
}
