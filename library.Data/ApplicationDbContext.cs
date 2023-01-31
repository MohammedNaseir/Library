using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace library.Data
{
    public class libraryDbContext : IdentityDbContext
    {
        public libraryDbContext(DbContextOptions<libraryDbContext> options)
            : base(options)
        {

        }
    }
}