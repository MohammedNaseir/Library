using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Core.ViewModels
{
    [Index(nameof(Email),IsUnique =true)]
    [Index(nameof(Username),IsUnique =true)]
    public class UserViewModel
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsDeleted { get; set; } 
        public DateTime CreatedOn { get; set; } 
        public DateTime? LastUpdatedOn { get; set; } 
    }
}
