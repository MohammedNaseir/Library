using library.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace library.Data
{
    public class libraryDbContext : IdentityDbContext
    {
        public libraryDbContext(DbContextOptions<libraryDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // add sequence for serial nubmer
            builder.HasSequence<int>("SerialNumber", schema: "shared")
               .StartsAt(1000001);
            builder.Entity<BookCopy>()
                .Property(e => e.SerialNumber)
                .HasDefaultValueSql("NEXT VALUE FOR shared.SerialNumber");

            //define key from 2 attr (compsite key)
            builder.Entity<BookCategory>().HasKey(e => new { e.BookId, e.CategoryId });
            base.OnModelCreating(builder);
        }

    }
}