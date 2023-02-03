using library.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Infrastructure.Services.Categories
{
    public interface ICategoryService
    {
        Task<CategoryVM> GetAll();

        Task<int> Create(CategoryVM categoryVM);

        Task<int> Update(CategoryVM categoryVM);

        Task<CategoryVM> Get(int Id);
        
         Task<List<CategoryVM>> GetCategoryList();

        Task<int> Delete(int Id);
    }
}
