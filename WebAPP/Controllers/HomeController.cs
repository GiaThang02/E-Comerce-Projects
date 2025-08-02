using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebAPP.Models;
using WebAPP.Models.Repository;
using WebAPP.Models.ViewModels;

namespace WebAPP.Controllers
{
    public class HomeController : Controller
    {
        public int PageSize = 6;

        private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _dataContext = context;
        }

        public async Task<IActionResult> Index(string searchString, string searchName, decimal? minPrice, decimal? maxPrice, int pageIndex = 1)
        {
            int pageSize = PageSize;
            var products = _dataContext.Products
                                        .Include(p => p.Category)
                                        .Include(p => p.Brand)
                                        .AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(searchName))
            {
                products = products.Where(p => p.Name.Contains(searchName));
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            var paginatedList = await PaginatedList<ProductModel>.CreateAsync(products, pageIndex, pageSize);
            var viewModel = new ProductListViewModel
            {
                Products = paginatedList,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageIndex,
                    TotalItems = await products.CountAsync(),
                    ItemsPerPage = pageSize
                }
            };

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentNameFilter"] = searchName;
            ViewData["CurrentMinPriceFilter"] = minPrice;
            ViewData["CurrentMaxPriceFilter"] = maxPrice;

            var sliders=_dataContext.Sliders.Where(s=>s.Status==1).ToList();
            ViewBag.Sliders = sliders;
            return View(viewModel);
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if (statuscode == 404)
            {
                return View("NotFound");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}
