using Microsoft.AspNetCore.Mvc;
using WebAPP.Models;
using WebAPP.Models.Repository;
using WebAPP.Models.ViewModels;

namespace WebAPP.ViewComponents
{
	public class CartSummaryViewComponent : ViewComponent
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CartSummaryViewComponent(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public IViewComponentResult Invoke()
		{
			var cartItems = _httpContextAccessor.HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			var cartSummary = new CartSummaryViewModel
			{
				ItemCount = cartItems.Sum(x => x.Quantity),
				TotalAmount = cartItems.Sum(x => x.Quantity * x.Price)
			};

			return View(cartSummary);
		}
	}
}
