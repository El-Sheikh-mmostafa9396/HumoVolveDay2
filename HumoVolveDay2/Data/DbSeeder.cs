using HumoVolveDay2.Models;

namespace HumoVolveDay2.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // If database already has data, skip seeding
            if (context.Category.Any() || context.Product.Any())
                return;

            var electronics = new Category { Name = "Electronics" };
            var books = new Category { Name = "Books" };
            var clothing = new Category { Name = "Clothing" };

            context.Category.AddRange(electronics, books, clothing);
            context.SaveChanges();

            var Product = new[]
            {
                new Product { Name = "Smartphone", Price = 699.99m, CategoryId = electronics.Id },
                new Product { Name = "Laptop", Price = 1199.50m, CategoryId = electronics.Id },
                new Product { Name = "Novel", Price = 14.99m, CategoryId = books.Id },
                new Product { Name = "T-Shirt", Price = 19.99m, CategoryId = clothing.Id }
            };

            context.Product.AddRange(Product);
            context.SaveChanges();
        }
    }
}
