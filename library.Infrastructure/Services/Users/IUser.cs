using library.Core.ViewModels;
using library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Infrastructure.Services.Users
{
    public interface IUser
    {
        public IEnumerable<UserViewModel> GetUsers();
    }
}
