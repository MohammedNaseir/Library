using library.Core.Constants;
using library.Infrastructure.Services.Authors;
using library.Infrastructure.Services.Books;

namespace library.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnviroment;
        private readonly IBookService _bookService;
        private List<string> _allowedExtension = new() { ".jpg", ".jpge"};
        private int _maxAllowedSize = 2097152; // 2mg 
        
        public BooksController(IBookService bookService, 
            IWebHostEnvironment webHostEnviroment)
        {
            _bookService = bookService;
            _webHostEnviroment = webHostEnviroment;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {           
            return View("Form", PopulateViweModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookFormVM model)
        {           
            if (!ModelState.IsValid)
            {
                return View("Form", PopulateViweModel(model));
            }
            // check image extentions and size 
            if(model.Image is not null)
            {
                var extension = Path.GetExtension(model.Image.FileName);
                if (!_allowedExtension.Contains(extension))
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.NotAllowedExtension);
                    return View("Form", PopulateViweModel(model));
                }
                if (model.Image.Length > _maxAllowedSize)
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.MaxSize);
                    return View("Form", PopulateViweModel(model));
                }
                var imageName = $"{Guid.NewGuid()}{extension}";
                var path = Path.Combine($"{_webHostEnviroment.WebRootPath}/images/books", imageName);
                using var stream = System.IO.File.Create(path);
                model.Image.CopyTo(stream);
                model.ImageUrl=imageName;
            }
            _bookService.Create(model);         
            return RedirectToAction(nameof(Index)); 
        }
        
        
        
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var book = _bookService.GetBook(id);
            if (book is null)
                return NotFound();

            var model =_bookService.EditBookGet(book);
            var viewModel = PopulateViweModel(model);
            viewModel.SelectedCategories=book.Categories.Select(c=>c.CategoryId).ToList();
            return View("Form", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookFormVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", PopulateViweModel(model));
            }
            var book = _bookService.GetBook(model.Id);
            if (book is null)
                return NotFound();

            // check image extentions and size 
            if (model.Image is not null)
            {
                if (!string.IsNullOrEmpty(book.ImageUrl))
                {
                    var oldImagePath = Path.Combine($"{_webHostEnviroment.WebRootPath}/images/books", book.ImageUrl);
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }
                var extension = Path.GetExtension(model.Image.FileName);
                if (!_allowedExtension.Contains(extension))
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.NotAllowedExtension);
                    return View("Form", PopulateViweModel(model));
                }
                if (model.Image.Length > _maxAllowedSize)
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.MaxSize);
                    return View("Form", PopulateViweModel(model));
                }
                var imageName = $"{Guid.NewGuid()}{extension}";
                var path = Path.Combine($"{_webHostEnviroment.WebRootPath}/images/books", imageName);
                using var stream = System.IO.File.Create(path);
                model.Image.CopyTo(stream);
                model.ImageUrl = imageName;
            }
            _bookService.Update(model);
            return RedirectToAction(nameof(Index));
        }

        private BookFormVM PopulateViweModel(BookFormVM? model=null)
        {
            BookFormVM viewModel = model is null ? new BookFormVM():model ;
            viewModel.Authors  = _bookService.GetAuthors();
            viewModel.Categories = _bookService.GetCategories();            
            return viewModel;       
        }
        public IActionResult AllowItem(BookFormVM model)
        {
            var book = _bookService.IsBookExists(model);
            var isAllowed = book is null || book.Id.Equals(model.Id);
            return Json(isAllowed);
        }
    }
}
