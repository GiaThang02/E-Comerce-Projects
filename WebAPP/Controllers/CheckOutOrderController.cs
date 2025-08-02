using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PayPal.Api;
using System.Security.Claims;
using WebAPP.Models;
using WebAPP.Models.Repository;
using WebAPP.Models.ViewModels;

namespace WebAPP.Controllers
{
    public class CheckOutOrderController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;

        public CheckOutOrderController(DataContext context, IConfiguration configuration)
        {
            _dataContext = context;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            var user = _dataContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");

            }

            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            decimal shippingPrice = 0;
            if (shippingPriceCookie != null)
            {
                var shippingPriceJson = shippingPriceCookie;
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
            }

            decimal grandTotal = cartItems.Sum(item => item.Quantity * item.Price) + shippingPrice;
       

            OrderViewModel cartVM = new()
            {
                UserName = user.UserName,
                Name = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Products = cartItems,
                ShippingCost = shippingPrice,
                GrandTotal = grandTotal
            };
            
            
            Response.Cookies.Append("GrandTotal", cartVM.GrandTotal.ToString("F2"));


            return View(cartVM);
        }


        [HttpPost]
        public async Task<IActionResult> GetShipping(string quan, string tinh, string phuong)
        {
            var existingShipping = await _dataContext.Shippings
                .FirstOrDefaultAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);

            decimal shippingPrice;

            if (existingShipping != null)
            {
                shippingPrice = existingShipping.Price;
            }
            else
            {
                shippingPrice = 50000; 
            }

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(10), 
                    Secure = true 
                };
            
                Response.Cookies.Append("ShippingPrice", shippingPrice.ToString(), cookieOptions); // Lưu giá trị là chuỗi


            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Tính tổng tiền (bao gồm phí vận chuyển)
            decimal grandTotal = cartItems.Sum(x => x.Quantity * x.Price) + shippingPrice;
            HttpContext.Session.SetString("GrandTotal", grandTotal.ToString("F2"));

            return Json(new { success = true, shippingPrice = shippingPrice, grandTotal = grandTotal });
        }



        [HttpGet]
        public IActionResult DeleteShippingCost()
        {
            // Xóa cookie phí vận chuyển
            Response.Cookies.Delete("ShippingPrice");
            return RedirectToAction("Index", "Cart");
        }


        [HttpPost]
        public IActionResult ApplyCoupon(string couponCode)
        {
            var coupon = _dataContext.Coupons.FirstOrDefault(c => c.Code == couponCode && c.IsActive);

            if (coupon == null)
            {
                TempData["CouponError"] = "Mã giảm giá không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("Index");
            }

            // Lưu mã giảm giá vào session
            HttpContext.Session.SetString("CouponCode", couponCode);

            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Tính toán GrandTotal từ giỏ hàng
            decimal grandTotal = cartItems.Sum(x => x.Quantity * x.Price);

            // Lấy phí vận chuyển từ cookie
            decimal shippingPrice = 0;
            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            if (!string.IsNullOrEmpty(shippingPriceCookie))
            {
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceCookie);
            }

            grandTotal += shippingPrice;

            decimal discountedTotal = grandTotal * (1 - coupon.DiscountPercentage / 100);

            // Lưu giá trị GrandTotal vào session
            HttpContext.Session.SetString("GrandTotal", discountedTotal.ToString("F2"));

            // Trả về kết quả
            return Json(new
            {
                success = true,
                shippingPrice = shippingPrice,
                grandTotal = discountedTotal,
                message = "Mã giảm giá đã được áp dụng thành công!"
            });
        }

        public async Task<IActionResult> Checkout()
        {
            var user = _dataContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");

            }
            string paymentMethod = Request.Form["PaymentMethod"];

            if (user.UserName == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {

                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var orderCode = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("OrderCode", orderCode);

                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

                var shippingPriceCookie = Request.Cookies["ShippingPrice"];
                decimal shippingPrice = 0;
                if (shippingPriceCookie != null)
                {
                    shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceCookie);
                }


                // Lấy giá trị GrandTotal từ cookie
                var grandTotalString = HttpContext.Session.GetString("GrandTotal");
                decimal grandTotal = 0;
                if (!string.IsNullOrEmpty(grandTotalString) && decimal.TryParse(grandTotalString, out grandTotal))
                {
                }

                var orderitem = new OrderModel
                {
                    OrderCode = orderCode,
                    UserName = user.Email,
                    Status = 1,
                    CreatedDate = DateTime.Now,
                    UserId = user.Id,
                    GrandTotal = grandTotal,
                    ShippingCost = shippingPrice
                };


                _dataContext.Orders.Add(orderitem);
                await _dataContext.SaveChangesAsync();

                foreach (var cart in cartItems)
                {
                    var orderdetails = new OrderDetails
                    {
                        UserName = user.UserName,
                        OrderCode = orderCode,
                        ProductId = cart.ProductID,
                        Price = cart.Price,
                        Quantity = cart.Quantity,
                        GrandTotal = cart.Quantity * cart.Price 
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
                if (paymentMethod == "PayPal")
                {
                    HttpContext.Session.SetString("OrderCode", orderCode);
                    HttpContext.Session.SetString("UserEmail", userEmail);
                    HttpContext.Session.SetString("UserId", userId);
                    HttpContext.Session.SetString("ShippingCost", shippingPrice.ToString());

                    // Xóa giỏ hàng và cookie
                    HttpContext.Session.Remove("Cart");
                    HttpContext.Session.Remove("CouponCode");
                    Response.Cookies.Delete("GrandTotal");
                    Response.Cookies.Delete("ShippingPrice");

                    // Gọi hàm tạo thanh toán
                    return CreatePayment(orderCode, grandTotal);

                }


                HttpContext.Session.Remove("Cart");
                HttpContext.Session.Remove("CouponCode");

                Response.Cookies.Delete("GrandTotal");
                Response.Cookies.Delete("ShippingPrice");

                TempData["success"] = "Đơn hàng đã được tạo thành công!";
                return RedirectToAction("Index", "Cart");
            }
        }


        public IActionResult CreatePayment(string orderCode, decimal grandTotal)
        {
            // Khởi tạo APIContext
            var apiContext = new APIContext(new OAuthTokenCredential(
                _configuration["PayPal:ClientId"],
                _configuration["PayPal:ClientSecret"]
            ).GetAccessToken())
            {
                Config = new Dictionary<string, string>
        {
            { "mode", _configuration["PayPal:Mode"] } // 'sandbox' hoặc 'live'
        }
            };

            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
             {
            new Transaction
            {
                description = "Order payment",
                invoice_number = orderCode,
                amount = new Amount
                {
                    currency = "USD", // Đổi sang loại tiền tệ nếu cần
                    total = grandTotal.ToString("F2")
                }
            }
        },
                redirect_urls = new RedirectUrls
                {
                    cancel_url = Url.Action("Cancel", "Checkout", null, Request.Scheme),
                    return_url = Url.Action("Success", "Checkout", null, Request.Scheme)
                }
            };

            var createdPayment = payment.Create(apiContext);
            return Redirect(createdPayment.links.First(l => l.rel == "approval_url").href);
        }

        public async Task<IActionResult> Success(string paymentId, string token)
        {
            var orderCode = HttpContext.Session.GetString("OrderCode");

            if (string.IsNullOrEmpty(orderCode))
            {
                return NotFound("Đơn hàng không tồn tại.");
            }

            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại.");
            }

            order.Status = 4;
            await _dataContext.SaveChangesAsync();

            // Xóa thông tin khỏi session
            HttpContext.Session.Remove("OrderCode");
            HttpContext.Session.Remove("UserEmail");
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("ShippingCost");

            TempData["success"] = "Thanh toán thành công!";
            return RedirectToAction("Index", "Cart");
        }





    }
}
