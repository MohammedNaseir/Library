using library.Core.ViewModels;
using library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Infrastructure.Services.Categories
{
    public interface ICategoryService
    {
        Task<Category> GetAll();
        Task<int> Create(CategoryVM Category);
        Task<int> Update(CategoryVM Category);
        Task<CategoryVM> Get(int Id);    
        Task<List<Category>> GetCategoryList();
        Task<int> Delete(int Id);
    }
}
