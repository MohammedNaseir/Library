using library.Core.ViewModels;
using library.Data.Models;
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

        public async Task<IActionResult> Index()
        {
            var Categories = await _categoryService.GetCategoryList();
            return View(Categories);
        }
        [HttpGet]
        public IActionResult Create() 
        { 
            return View("Form");
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //var category = new Category { Name=model.Name };
            await _categoryService.Create(model);           
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.Get(id);
            if (category == null)
            {
                return NotFound();
            }
            return View("Form", category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryVM model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            var category = _categoryService.Update(model);

            if (category.Result == -1)
                return NotFound();
            return RedirectToAction(nameof(Index));
        }

    }
}
