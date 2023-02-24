using library.Core.ViewModels;
using library.Data.Models;
using library.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace library.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var Categories = await _categoryService.GetCategoryList();
            return View(Categories);
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Create() 
        {
            return PartialView("_Form");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryVM model)
        {
            if (!ModelState.IsValid)          
                return BadRequest();  
            //var category = new Category { Name=model.Name };
            await _categoryService.Create(model);           
            return PartialView("_CategoryRow", model);
        }
        
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.Get(id);
            if (category == null)
            {
                return NotFound();
            }
            return PartialView("_Form", category);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var category = _categoryService.Update(model);

            if (category.Result == -1)
                return NotFound();
            return PartialView("_CategoryRow", category);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var category = _categoryService.GetCategory(id);
            if (category is null)
                return NotFound();
            category.IsDeleted = !category.IsDeleted;
            category.LastUpdatedOn = DateTime.Now;
            return Ok(category.LastUpdatedOn.ToString());
        }
    }
}
