using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using WebAPP.Models.Repository;
using WebAPP.Models;
using Microsoft.EntityFrameworkCore;

namespace WebAPP.Controllers
{
    public class CheckoutCOD : Controller
    {
        private readonly DataContext _dataContext;

        public CheckoutCOD(DataContext _context)
        {
            _dataContext = _context;

        }
        public async Task<IActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);  // nếu có đăng nhập mới có UserEmail
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var ordercode = Guid.NewGuid().ToString();
                var orderitem = new OrderModel
                {
                    OrderCode = ordercode,
                    UserName = userEmail,
                    Status = 1,
                    CreatedDate = DateTime.Now,
                    UserId = userId
                };

                // Lấy giỏ hàng từ session
                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

                // Lấy giá trị ShippingCost từ cookie
                var shippingPriceCookie = Request.Cookies["ShippingPrice"];
                decimal shippingPrice = 0;
                if (shippingPriceCookie != null)
                {
                    shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceCookie);
                }

                // Lấy giá trị GrandTotal từ cookie
                var grandTotalCookie = Request.Cookies["GrandTotal"];
                decimal grandTotal = 0;
                if (grandTotalCookie != null)
                {
                    grandTotal = JsonConvert.DeserializeObject<decimal>(grandTotalCookie);
                }

                string couponCode = HttpContext.Session.GetString("CouponCode");
                if (!string.IsNullOrEmpty(couponCode))
                {
                    var coupon = _dataContext.Coupons.FirstOrDefault(c => c.Code == couponCode && c.IsActive && c.ExpirationDate > DateTime.Now);
                    if (coupon != null)
                    {
                        grandTotal *= (1 - coupon.DiscountPercentage / 100);
                    }
                }

                // Lưu GrandTotal vào Order
                orderitem.GrandTotal = grandTotal;
                orderitem.ShippingCost = shippingPrice;

                _dataContext.Orders.Add(orderitem);
                await _dataContext.SaveChangesAsync();

                foreach (var cart in cartItems)
                {
                    var orderdetails = new OrderDetails
                    {
                        UserName = userEmail,
                        OrderCode = ordercode,
                        ProductId = cart.ProductID,
                        Price = cart.Price,
                        Quantity = cart.Quantity,
                        GrandTotal = cart.Quantity * cart.Price // Lưu GrandTotal cho mỗi sản phẩm
                    };

                    // Cập nhật số lượng và trạng thái bán hàng của sản phẩm
                    var product = await _dataContext.Products.Where(p => p.Id == cart.ProductID).FirstOrDefaultAsync();
                    if (product != null)
                    {
                        product.Quantity -= cart.Quantity; // Giảm số lượng sản phẩm
                        product.Sold += cart.Quantity; // Cập nhật số lượng đã bán
                        _dataContext.Products.Update(product);
                    }

                    // Lưu OrderDetails
                    _dataContext.OrderDetails.Add(orderdetails);
                    await _dataContext.SaveChangesAsync();
                }

                // Xóa giỏ hàng và mã giảm giá khỏi session sau khi đặt hàng thành công
                HttpContext.Session.Remove("Cart");
                HttpContext.Session.Remove("CouponCode");

                // Xóa cookie GrandTotal và ShippingPrice
                Response.Cookies.Delete("GrandTotal");
                Response.Cookies.Delete("ShippingPrice");

                TempData["success"] = "Đơn hàng đã được tạo thành công!";
                return RedirectToAction("Index", "Cart");
            }
        }

    }
}
