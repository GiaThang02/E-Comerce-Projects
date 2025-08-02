using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPP.Models;
using WebAPP.Models.Repository;
using WebAPP.Models.Services;
using WebAPP.Models.ViewModels;

namespace WebAPP.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;

        public CheckoutController(DataContext context, IConfiguration configuration)
        {
            _dataContext = context;
            _configuration = configuration;
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

            // Tạo đối tượng thanh toán cho PayPal
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
            // Kiểm tra và lưu thông tin đơn hàng
            var orderCode = HttpContext.Session.GetString("OrderCode");
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shippingCost = Convert.ToDecimal(HttpContext.Session.GetString("ShippingCost"));
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Lưu đơn hàng
            var existingOrder = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
            if (existingOrder == null)
            {
                // Lưu đơn hàng
                var orderItem = new OrderModel
                {
                    OrderCode = orderCode,
                    UserName = userEmail,
                    ShippingCost = shippingCost,
                    Status = 1,
                    CreatedDate = DateTime.Now,
                    UserId = userId,
                    GrandTotal = Convert.ToDecimal(HttpContext.Session.GetString("GrandTotal"))
                };

                _dataContext.Add(orderItem);
                await _dataContext.SaveChangesAsync();
            }
            // Lưu chi tiết đơn hàng
            foreach (var cart in cartItems)
            {
                var orderDetails = new OrderDetails
                {
                    UserName = userEmail,
                    OrderCode = orderCode,
                    ProductId = cart.ProductID,
                    Price = cart.Price,
                    Quantity = cart.Quantity
                };

                // Cập nhật thông tin sản phẩm
                var product = await _dataContext.Products.FirstOrDefaultAsync(p => p.Id == cart.ProductID);
                if (product != null)
                {
                    product.Quantity -= cart.Quantity;
                    product.Sold += cart.Quantity;

                    _dataContext.Update(product);
                }

                _dataContext.Add(orderDetails);
            }

            await _dataContext.SaveChangesAsync();

            // Xóa giỏ hàng
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Đơn hàng đã được tạo thành công. Vui lòng chờ để duyệt đơn hàng.";
            return RedirectToAction("Index", "Cart");
        }

        public IActionResult Cancel()
        {
            // Xử lý khi thanh toán bị hủy
            TempData["error"] = "Bạn đã hủy thanh toán.";
            return RedirectToAction("Index", "Cart");
        }

    }
}
