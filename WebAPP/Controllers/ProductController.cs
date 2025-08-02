using Microsoft.AspNetCore.Mvc;
using WebAPP.Models.Repository;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAPP.Models.ViewModels;
using WebAPP.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebAPP.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;

        public ProductController(DataContext context)
        {
            _dataContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int Id)
        {
            
            if (Id <= 0) return RedirectToAction("Index");

            // Buộc phải gộp do ở phần productdetails có phần rating nữa
            var productById = _dataContext.Products.Include(p=>p.Ratings).Where(c => c.Id == Id).FirstOrDefault();

            if (productById == null) return RedirectToAction("Index");

            //Related product:
            var related_product=await _dataContext.Products.Where(p=>p.CategoryId==productById.CategoryId && p.Id !=productById.Id)
                .Take(4).ToListAsync();

            
             ViewBag.RelatedProducts=related_product;



            //Chỗ này ở phần details rating
            var viewModel = new ProductDetailsViewModel
            {
                ProductDetails = productById,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentProduct(RatingModel rating)
        {
            if(ModelState.IsValid)
            {
                var ratingEntity = new RatingModel
                {
					ProductId = rating.ProductId,
                    Name = rating.Name,
                    Email = rating.Email,
                    Comment = rating.Comment,
                    Star = rating.Star
                };

                _dataContext.Ratings.Add(ratingEntity);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm đánh giá thành công";
                return Redirect(Request.Headers["Referer"]);

			}
            else
            {

				TempData["error"] = "CÓ một vài thứ đang bị lỗi !";
				List<String> errors = new List<String>();
				foreach (var value in ModelState.Values)
				{

					foreach (var error in value.Errors)
					{
						errors.Add(error.ErrorMessage);
					}

				}
				string errorMessage = string.Join("\n", errors);
				return RedirectToAction("Detail", new {id=rating.ProductId});

			}
			return RedirectToAction("Detail", "Product");

		}
	}
}
