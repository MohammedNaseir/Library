using library.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace library.Data
{
    public class libraryDbContext : IdentityDbContext<ApplicationUser>
    {
        public libraryDbContext(DbContextOptions<libraryDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Governorate> Governorates { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<RentalCopy> RentalCopies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // add sequence for serial nubmer
            builder.HasSequence<int>("SerialNumber", schema: "shared")
               .StartsAt(1000001);
            
            builder.Entity<BookCopy>()
                .Property(e => e.SerialNumber)
                .HasDefaultValueSql("NEXT VALUE FOR shared.SerialNumber");

			var cascadeFKs = builder.Model.GetEntityTypes()
			  .SelectMany(t => t.GetForeignKeys())
			  .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);

			foreach (var fk in cascadeFKs)
				fk.DeleteBehavior = DeleteBehavior.Restrict;


			//define key from 2 attr (compsite key)
			builder.Entity<BookCategory>().HasKey(e => new { e.BookId, e.CategoryId });
			builder.Entity<RentalCopy>().HasKey(e => new { e.RentalId, e.BookCopyId });
            builder.Entity<Rental>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<RentalCopy>().HasQueryFilter(e => !e.Rental!.IsDeleted);
            base.OnModelCreating(builder);

            //change tables name
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable(name: "UserRoles");

            //remove colimn from table in identity
            //builder.Entity<IdentityUser>().Ignore(e=>e.PhoneNumber)
            //                               .Ignore(e=>e.PhoneNumberConfirmed);

        }

    }
}