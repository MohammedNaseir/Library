using library.Infrastructure.Services.Authors;

namespace library.Web.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorService _authorService;
        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var Authors = await _authorService.GetAuthorList();
            return View(Authors);
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuthorFormVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var viewModel = await _authorService.Create(model);

            return PartialView("_AuthorRow", viewModel);
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _authorService.Get(id);
            if (author == null)
            {
                return NotFound();
            }
            return PartialView("_Form", author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AuthorFormVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var author = await _authorService.Update(model);
            return PartialView("_AuthorRow", author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var author = _authorService.GetAuthor(id);
            if (author is null)
                return NotFound();
            author.IsDeleted = !author.IsDeleted;
            author.LastUpdatedOn = DateTime.Now;
            _authorService.SaveChanges();
            return Ok(author.LastUpdatedOn.ToString());
        }
        public IActionResult AllowItem(AuthorFormVM model)
        {
            var author = _authorService.IsAuthorExists(model);
            var isAllowed = author is null || author.Id.Equals(model.Id);
            return Json(isAllowed);
        }
    }
}
