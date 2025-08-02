using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using WebAPP.Models;
using WebAPP.Models.Repository;
using WebAPP.Models.ViewModels;

namespace WebAPP.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;

        public CartController(DataContext context)
        {
            _dataContext = context;
        }

        // Hiển thị giỏ hàng
        public IActionResult Index()
        {
            // Lấy danh sách sản phẩm trong giỏ hàng từ session
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            decimal shippingPrice = 0;
            if(shippingPriceCookie != null)
            {
                var shippingPriceJson = shippingPriceCookie;
                shippingPrice=JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
            }

            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                ShippingCost = shippingPrice,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price) + shippingPrice
            };

            string couponCode = HttpContext.Session.GetString("CouponCode");
            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = _dataContext.Coupons.FirstOrDefault(c => c.Code == couponCode && c.IsActive && c.ExpirationDate > DateTime.Now);
                if (coupon != null)
                {
                    cartVM.GrandTotal *= (1 - coupon.DiscountPercentage / 100);
                    cartVM.CouponCode = couponCode;
                }
                else
                {
                    HttpContext.Session.Remove("CouponCode");
                }
            }
            var grandTotalJson = JsonConvert.SerializeObject(cartVM.GrandTotal);
            Response.Cookies.Append("GrandTotal", grandTotalJson, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                HttpOnly = true,
                Secure = true
            });

            return View(cartVM);
        }


        // Hiển thị trang thanh toán
        public IActionResult Checkout()
        {
            return View("~/Views/Checkout/Index.cshtml");
        }

        // Thêm sản phẩm vào giỏ hàng
    
        public async Task<IActionResult> Add(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItems = cart.FirstOrDefault(c => c.ProductID == Id);
            if (cartItems == null)
            {
                cart.Add(new CartItemModel(product));
            }
            else
            {
                cartItems.Quantity += 1;
            }
            HttpContext.Session.SetJson("Cart", cart);

            TempData["success"] = "Thêm sản phẩm thành công";

            return Redirect(Request.Headers["Referer"].ToString());
        }

        // Giảm số lượng sản phẩm trong giỏ hàng
        public IActionResult Decrease(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.FirstOrDefault(c => c.ProductID == Id);
            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity -= 1;
                }
                else
                {
                    cart.Remove(cartItem);
                }
            }
            HttpContext.Session.SetJson("Cart", cart);
            return RedirectToAction("Index");
        }

        // Tăng số lượng sản phẩm trong giỏ hàng
        public async Task<IActionResult> Increase(int Id)
        {
            ProductModel product=await _dataContext.Products.Where(p=>p.Id==Id).FirstOrDefaultAsync();
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(c=>c.ProductID== Id).FirstOrDefault();
            if (cartItem.Quantity>=1 && product.Quantity>cartItem.Quantity)
            {
                ++cartItem.Quantity;
                TempData["success"] = "Tăng thành công ";

            }
            else
            {
                cartItem.Quantity = product.Quantity;
                TempData["success"] = "Đã đạt số lượng tối đa cho phép ";
            }
            if (cart.Count==0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);

            }
            TempData["success"] = "Tăng thành công ";
            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public IActionResult Remove(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            cart.RemoveAll(p => p.ProductID == Id);
            HttpContext.Session.SetJson("Cart", cart);
            return RedirectToAction("Index");
        }

        // Xóa toàn bộ giỏ hàng
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Xóa giỏ hàng thành công!";
            return RedirectToAction("Index");
        }


   
    }
}
