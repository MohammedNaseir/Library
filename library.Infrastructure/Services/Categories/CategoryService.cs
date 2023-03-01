using AutoMapper;
using library.Core.Exceptions;
using library.Core.ViewModels;
using library.Data;
using library.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace library.Infrastructure.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly libraryDbContext _db;
        public CategoryService(libraryDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<CategoryViewModel> Create(CategoryVM Category)
        {

            var category = _mapper.Map<Category>(Category);
            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();
            var viewModel = _mapper.Map<CategoryViewModel>(category);
            return viewModel;
        }

        public Category GetCategory(int Id)
        {
            var category = _db.Categories.FirstOrDefault(c => c.Id == Id);
            return category;
        }

        public async Task<CategoryVM> Get(int Id)
        {
            var category = await _db.Categories.SingleOrDefaultAsync(x => x.Id == Id && !x.IsDeleted);
            if (category == null)
            {
                throw new EntityNotFoundException();
            }
            var categoryVM = _mapper.Map<CategoryVM>(category);
            //CategoryVM categoryVM = new CategoryVM
            //{
            //    Id = category.Id,
            //    Name = category.Name
            //};
            return (categoryVM);
        }


        public async Task<IEnumerable<CategoryViewModel>> GetCategoryList()
        {
            var categories = await _db.Categories
                .AsNoTracking()
                .ToListAsync();
            var viewModel = _mapper.Map<IEnumerable<CategoryViewModel>>(categories);
            return (viewModel);
        }

        public async Task<CategoryViewModel> Update(CategoryVM model)
        {
            var category = await _db.Categories.FindAsync(model.Id);
            category = _mapper.Map(model, category);
            //category.Name = model.Name;
            category.LastUpdatedOn = DateTime.Now;
            await _db.SaveChangesAsync();
            var viewModel = _mapper.Map<CategoryViewModel>(category);
            return viewModel;
        }

        public Category IsCategoryExists(CategoryVM category)
        {
            return _db.Categories.SingleOrDefault(x => x.Name == category.Name);
        }
    }
}
