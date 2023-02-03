using AutoMapper;
using library.Core.ViewModels;
using library.Data;
using library.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        Task<int> ICategoryService.Create(CategoryVM categoryVM)
        {
            throw new NotImplementedException();
        }

        Task<int> ICategoryService.Delete(int Id)
        {
            throw new NotImplementedException();
        }

        Task<CategoryVM> ICategoryService.Get(int Id)
        {
            throw new NotImplementedException();
        }

        Task<CategoryVM> ICategoryService.GetAll()
        {
            throw new NotImplementedException();
        }

        Task<List<CategoryVM>> ICategoryService.GetCategoryList()
        {
            throw new NotImplementedException();
        }

        Task<int> ICategoryService.Update(CategoryVM categoryVM)
        {
            throw new NotImplementedException();
        }
    }
}
