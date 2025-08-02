using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebAPP.Models;
using WebAPP.Models.Repository;

namespace WebAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CouponsController : Controller
    {
        private readonly DataContext _context;

        public CouponsController(DataContext context)
        {
            _context = context;
        }

        // Danh sách mã giảm giá
        [HttpGet]
        public IActionResult CouponList()
        {
            var coupons = _context.Coupons.ToList();
            return View(coupons);
        }

        // Hiển thị form tạo mã giảm giá
        [HttpGet]
        public IActionResult CreateCoupon()
        {
            return View();
        }

        // Xử lý yêu cầu tạo mã giảm giá
        [HttpPost]
        public async Task<IActionResult> CreateCoupon(Coupon model)
        {
            if (ModelState.IsValid)
            {
                model.Code = GenerateCouponCode();
                model.IsActive = true;

                _context.Coupons.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("CouponList");
            }
            return View(model);
        }

        // Hiển thị form chỉnh sửa mã giảm giá
        [HttpGet]
        public IActionResult EditCoupon(int id)
        {
            var coupon = _context.Coupons.Find(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        // Xử lý yêu cầu chỉnh sửa mã giảm giá
        [HttpPost]
        public async Task<IActionResult> EditCoupon(int id, Coupon model)
        {
            if (ModelState.IsValid)
            {
                var coupon = _context.Coupons.Find(id);
                if (coupon == null)
                {
                    return NotFound();
                }

                coupon.Code = model.Code;
                coupon.DiscountPercentage = model.DiscountPercentage;
                coupon.ExpirationDate = model.ExpirationDate;
                coupon.IsActive = model.IsActive;

                _context.Coupons.Update(coupon);
                await _context.SaveChangesAsync();
                return RedirectToAction("CouponList");
            }
            return View(model);
        }

        // Xóa mã giảm giá
        [HttpPost]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var coupon = _context.Coupons.Find(id);
            if (coupon == null)
            {
                return NotFound();
            }

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            return RedirectToAction("CouponList");
        }

        private string GenerateCouponCode()
        {
            return Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }
    }
}
