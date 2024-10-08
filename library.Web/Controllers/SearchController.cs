﻿using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace library.Web.Controllers
{
	public class SearchController : Controller
	{
		private readonly libraryDbContext _context;
		private readonly IMapper _mapper;
		//private readonly IHashids _hashids;
		private readonly IDataProtector _dataProtector;

		public SearchController(libraryDbContext context, IMapper mapper,
			  IDataProtectionProvider dataProtector/*IHashids hashids*/)
		{
			_context = context;
			_mapper = mapper;
			_dataProtector = dataProtector.CreateProtector("Search");

			//_hashids = hashids;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Find(string query)
		{
			var books = _context.Books
				.Include(b => b.Author)
				.Where(b => !b.IsDeleted && (b.Title.Contains(query) || b.Author!.Name.Contains(query)))
				.Select(b => new { b.Title, Author = b.Author!.Name, Key = _dataProtector.Protect(b.Id.ToString()) })
				.ToList();

			return Ok(books);
		}

		public IActionResult Details(string bKey)
		{
			var bookId = _dataProtector.Unprotect(bKey);

			if (bookId.Length == 0)
				return NotFound();

			var book = _context.Books
				.Include(b => b.Author)
				.Include(b => b.Copies)
				.Include(b => b.Categories)
				.ThenInclude(c => c.Category)
				.SingleOrDefault(b => b.Id == int.Parse(bookId) && !b.IsDeleted);

			if (book is null)
				return NotFound();

			var viewModel = _mapper.Map<BookViewModel>(book);

			return View(viewModel);
		}

	}
}
