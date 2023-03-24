using library.Core.ViewModels;
using library.Data.Models;

namespace library.Infrastructure.Services.Categories
{
    public interface ICategoryService
    {
        Task<CategoryViewModel> Create(CategoryVM Category);
        Task<CategoryViewModel> Update(CategoryVM model);
        Task<CategoryVM> Get(int Id);
        Task<IEnumerable<CategoryViewModel>> GetCategoryList();
        Category GetCategory(int Id);
        Category IsCategoryExists(CategoryVM category);
        void SaveChanges();
    }
}
