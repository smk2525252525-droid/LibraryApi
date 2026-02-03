using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(LibraryDbContext context)
        {
            context.Database.Migrate();

            // 1. Seed Roles
            if (!context.Roles.Any())
            {
                var adminRole = new Role { Name = "Admin" };
                var memberRole = new Role { Name = "Member" };

                context.Roles.AddRange(adminRole, memberRole);
                context.SaveChanges();
            }

            // 2. Seed Admin User
            if (!context.Users.Any())
            {
                var adminRole = context.Roles.First(r => r.Name == "Admin");

                var adminUser = new User
                {
                    Name = "Kumail Hasan",
                    Email = "kumailhasanrizvi@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), // Hash the password, 
                    RoleId = adminRole.Id
                    // Note: No need to initialize 'Issuings' here if we removed 'required'
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }

            // 3. Seed Categories
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Fiction" },
                    new Category { Name = "Science" }
                );
                context.SaveChanges();
            }

            // 4. Seed Books
            if (!context.Books.Any())
            {
                // Get the actual Category objects to use their IDs
                var fiction = context.Categories.First(c => c.Name == "Fiction");
                var science = context.Categories.First(c => c.Name == "Science");

                var book1 = new Book
                {
                    Title = "The Martian",
                    Author = "Andy Weir",
                    CategoryId = fiction.Id, // using CategoryId  instead of Category (string) now
                    Status = "Available"
                };

                var book2 = new Book
                {
                    Title = "Sapiens",
                    Author = "Yuval Noah Harari",
                    CategoryId = science.Id, // usimg CategoryId  instead of Category (string) now
                    Status = "Available"
                };

                context.Books.AddRange(book1, book2);
                context.SaveChanges();
            }
        }
    }
}