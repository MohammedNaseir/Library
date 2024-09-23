

using AutoMapper;
using Microsoft.AspNetCore.DataProtection;

namespace library.Web.Controllers
{
    //[Authorize(Roles =AppRoles.Admin)]
    public class HomeController : Controller
    {
        private readonly libraryDbContext _db;
        private readonly IMapper _mapper;
		//private readonly IHashids _hashids;
		private readonly IDataProtector _dataProtector;

		public HomeController(libraryDbContext db, IMapper mapper,
			IDataProtectionProvider dataProtector/*IHashids hashids*/)
		{
			_db = db;
			_mapper = mapper;
			_dataProtector = dataProtector.CreateProtector("Search");

		}



		public IActionResult Index()
        {
			if(User.Identity!.IsAuthenticated)			
				return RedirectToAction(nameof(Index), "Dashboard");
			
			var Books = _db.Books.Where(x => !x.IsDeleted)
							  .Include(a => a.Author)
							  .OrderByDescending(x => x.Id)
							  .Take(10)
							  .ToList();
			var lst = _mapper.Map<IEnumerable<BookViewModel>>(Books);
			foreach (var book in lst)
			{
				book.Key = _dataProtector.Protect(book.Id.ToString());
				//book.Key = _hasohids.EncodeHex(book.Id.ToString()); 
			}
			return View(lst);
        }
    }
}