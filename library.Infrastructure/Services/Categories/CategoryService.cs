using AutoMapper;
using library.Core.Exceptions;
using library.Core.ViewModels;
using library.Data;
using library.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace library.Infrastructure.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly libraryDbContext _db;
        public CategoryService(libraryDbContext db)
        {
            _db = db;
        }

        public async Task<int> Create(CategoryVM Category)
        {
           Category category = new Category { Name = Category.Name };
           await _db.Categories.AddAsync(category);
           await _db.SaveChangesAsync();
           return 1;
        }

        public Category GetCategory(int Id)
        {
            var category =  _db.Categories.FirstOrDefault(c => c.Id == Id);
            return category;
        }

        public async Task<CategoryVM> Get(int Id)
        {
            var category = await _db.Categories.SingleOrDefaultAsync(x => x.Id == Id && !x.IsDeleted);
            if (category == null)
            {
                throw new EntityNotFoundException();
            }
            CategoryVM categoryVM = new CategoryVM
            {
                Id = category.Id,
                Name = category.Name
            };
            return (categoryVM);
        }

        public Task<Category> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Category>> GetCategoryList()
        {
            var categories = await _db.Categories.AsNoTracking().ToListAsync();
            //if (categories == null)
            //{
            //    throw new EntityNotFoundException();
            //}
            return (categories);
        }

        public async Task<int> Update(CategoryVM Category)
        {
            var category = _db.Categories.Find(Category.Id);

            if (category is null)
                return -1;

            category.Name = Category.Name;
            category.LastUpdatedOn = DateTime.Now;

            await _db.SaveChangesAsync();
            return Category.Id;
        }
    }
}
