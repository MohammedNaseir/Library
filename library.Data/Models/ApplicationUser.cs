using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Data.Models
{
    public class ApplicationUser :IdentityUser
    {
        [MaxLength(100)]
        public bool IsDeleted { get; set; }
        public string FullName { get; set; } = null!;
        public string? CreatedById { get; set; }       
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastUpdatedOn { get; set; }
        public string? LastUpdatedById { get; set; }
    }
}
