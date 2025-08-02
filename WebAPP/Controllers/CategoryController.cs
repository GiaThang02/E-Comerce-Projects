using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPP.Models;
using WebAPP.Models.Repository;

namespace WebAPP.Controllers
{
	public class CategoryController : Controller
	{
		private readonly DataContext _dataContext;
		public CategoryController(DataContext context)
		{
			_dataContext = context;
		}
		public async Task<IActionResult> Index( string Slug=" ")
		{
			CategoryModel category = _dataContext.Categories.Where(c=>c.Slug==Slug).FirstOrDefault();
			if(category == null) return RedirectToAction("Index");

			var productbyCategory= _dataContext.Products.Where(c => c.CategoryId == category.Id);
			return View(await productbyCategory.OrderByDescending(p => p.Id).ToListAsync());
		}
	}
}
