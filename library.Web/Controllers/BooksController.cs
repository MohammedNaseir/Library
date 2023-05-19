using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using library.Core.Constants;
using library.Data.Models;
using library.Infrastructure.Services.Authors;
using library.Infrastructure.Services.Books;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Linq.Dynamic.Core;

namespace library.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnviroment;
        private readonly IBookService _bookService;
        private List<string> _allowedExtension = new() { ".jpg", ".jpge"};
        private int _maxAllowedSize = 2097152; // 2mg 
        private readonly Cloudinary _cloudinary;

        public BooksController(IBookService bookService,
            IWebHostEnvironment webHostEnviroment,
            IOptions<CloudinarySettings> cloudinary)
        {
            _bookService = bookService;
            _webHostEnviroment = webHostEnviroment;
            Account account = new()
            {
                Cloud = cloudinary.Value.Cloud,
                ApiKey = cloudinary.Value.ApiKey,
                ApiSecret = cloudinary.Value.ApiSecret
            };
            _cloudinary = new Cloudinary(account);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost] 
        public IActionResult GetBooks()
        {
            var skip = int.Parse(Request.Form["start"]);         
            var pageSize = int.Parse(Request.Form["length"]);
           
            var serarcValue = Request.Form["search[value]"];
            
            var sortColumnIndex = Request.Form[ "order[0][column]"];
            var sortColumn = Request.Form[$"columns[{sortColumnIndex}][name]"];
            var sortColumnDirection = Request.Form["order[0][dir]"];

            IQueryable<Book> books = _bookService.GetBooks();
            if(!string.IsNullOrEmpty(serarcValue))
                books = books.Where(x => x.Title.Contains(serarcValue) || x.Author!.Name.Contains(serarcValue));
            books = books.OrderBy($"{sortColumn} {sortColumnDirection}");
            var data = books.Skip(skip).Take(pageSize).ToList();
            var mappedData = _bookService.BookMap(data);
            var recordsTotal = books.Count();
            var jsonData = new
            {                     
                recordsFiltered = recordsTotal,
                recordsTotal,
                data = mappedData
            };
            return Ok(jsonData);
        }
        public IActionResult Details(int id)
        {
            var bookVM = _bookService.GetBookViewModel(id);
            if (bookVM is null)
                return NotFound();
            return View(bookVM);
        }

        [HttpGet]
        public IActionResult Create()
        {           
            return View("Form", PopulateViweModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormVM model)
        {           
            if (!ModelState.IsValid)
            {
                return View("Form", PopulateViweModel(model));
            }
            // check image extentions and size 
            if (model.Image is not null)
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
                var thumbPath = Path.Combine($"{_webHostEnviroment.WebRootPath}/images/books/thumb/", imageName);
                
                using var stream = System.IO.File.Create(path);
                await model.Image.CopyToAsync(stream);
                stream.Dispose();
                model.ImageUrl = $"/images/books/{imageName}"; 
                model.ImageThumbnailUrl = $"/images/books/thumb/{imageName}";

                using var image = Image.Load(model.Image.OpenReadStream());
                var ratio = (float)image.Width / 200;
                var height = image.Height / ratio;
                image.Mutate(i => i.Resize(200, 300));
                image.Save(thumbPath);
                
                ////Cloudinary
                //using var straem = model.Image.OpenReadStream();

                //var imageParams = new ImageUploadParams
                //{
                //    File = new FileDescription(imageName, straem),
                //    UseFilename = true
                //};
                //var result = await _cloudinary.UploadAsync(imageParams);
                //model.ImageUrl = result.SecureUrl.ToString();
                //model.ImageThumbnailUrl = GetThumbnailUrl(model.ImageUrl);
                //model.ImagePublicID = result.PublicId;
                //model.ImageUrl = result.SecureUrl.ToString();

            }
            //else if (!string.IsNullOrEmpty(model.ImageUrl))
            //    model.ImageUrl = boo;

            int BookId = _bookService.Create(model);         
            return RedirectToAction(nameof(Details),new { id= BookId}); 
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
        public async Task<IActionResult> Edit(BookFormVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", PopulateViweModel(model));
            }
            var book = _bookService.GetBook(model.Id);
            if (book is null)
                return NotFound();
            string imageThumbnail = book.ImageThumbnailUrl;
            string imageBook = book.ImageUrl;
            // check image extentions and size 
            if (model.Image is not null)
            {
                if (!string.IsNullOrEmpty(book.ImageUrl))
                {
                    var oldImagePath = $"{_webHostEnviroment.WebRootPath}{book.ImageUrl}";
                    var oldthumpPath = $"{_webHostEnviroment.WebRootPath}{book.ImageThumbnailUrl}";
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);

                    if (System.IO.File.Exists(oldthumpPath))
                        System.IO.File.Delete(oldthumpPath);
                    ////cload
                    //await _cloudinary.DeleteResourcesAsync(book.ImagePublicID);
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

                //local
                var imageName = $"{Guid.NewGuid()}{extension}";
                var path = Path.Combine($"{_webHostEnviroment.WebRootPath}/images/books", imageName);
                var thumbPath = Path.Combine($"{_webHostEnviroment.WebRootPath}/images/books/thumb/", imageName);

                using var stream = System.IO.File.Create(path);
                await model.Image.CopyToAsync(stream);
                stream.Dispose();
                
                model.ImageUrl = $"/images/books/{imageName}";
                model.ImageThumbnailUrl = $"/images/books/thumb/{imageName}";

                using var image = Image.Load(model.Image.OpenReadStream());
                var ratio = (float)image.Width / 200;
                var height = image.Height / ratio;
                image.Mutate(i => i.Resize(width:200,height:(int)height));
                image.Save(thumbPath);


                ////Cloudinary
                //var imageName = $"{Guid.NewGuid()}{extension}";
                //using var straem = model.Image.OpenReadStream();
                //var imageParams = new ImageUploadParams
                //{
                //    File = new FileDescription(imageName, straem),
                //    UseFilename = true
                //};
                //var result = await _cloudinary.UploadAsync(imageParams);
                //model.ImageUrl = result.SecureUrl.ToString();
                //model.ImageThumbnailUrl = GetThumbnailUrl(model.ImageUrl);
                //model.ImagePublicID = result.PublicId;


            }
            else if (model.ImageUrl == null)
            {
                  model.ImageUrl = imageBook;
                  model.ImageThumbnailUrl = imageThumbnail;

            }

            _bookService.Update(model);
            return RedirectToAction(nameof(Details), new { id = book.Id });
        }

        private BookFormVM PopulateViweModel(BookFormVM? model=null)
        {
            BookFormVM viewModel = model is null ? new BookFormVM():model ;
            viewModel.Authors  = _bookService.GetAuthors();
            viewModel.Categories = _bookService.GetCategories();            
            return viewModel;       
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var book = _bookService.GetBook(id);
            if (book is null)
                return NotFound();
            book.IsDeleted = !book.IsDeleted;
            book.LastUpdatedOn = DateTime.Now;

            _bookService.SaveChanges();
            return Ok();
        }
        public IActionResult AllowItem(BookFormVM model)
        {
            var book = _bookService.IsBookExists(model);
            var isAllowed = book is null || book.Id.Equals(model.Id);
            return Json(isAllowed);
        }
        private string GetThumbnailUrl(string url)
        {
            //https://res.cloudinary.com/decm7aqke/image/upload/c_thumb,w_200,g_face/v1680050199/7e4ba894-487d-471d-92f5-10a00705c305_ohgiz6.jpg
            var separator = "image/upload";
            var urlParts=url.Split(separator);           
            var thumbnailUrl = $"{urlParts[0]}{separator}/c_thumb,w_200,g_face/{urlParts[1]}";
            return thumbnailUrl;
        }
    }
}
