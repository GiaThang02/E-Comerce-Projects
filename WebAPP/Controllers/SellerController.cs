using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAPP.Models;
using WebAPP.Models.Repository;

namespace WebAPP.Controllers
{
    public class SellerController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SellerController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async  Task<IActionResult> Index()
        {
            // Lấy UserId của người dùng hiện tại
           

            return View();
        }

        public async Task<IActionResult> ManagerProduct() 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var products = await _dataContext.Products.Where(p => p.UserId == userId).Include(p => p.Brand).Include(p => p.Category).ToListAsync();


            return View(products); 
        }
     

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {

            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            product.UserId = userId;

            if (ModelState.IsValid)
            {
                if (product.ImageUpload != null)
                {
                    //detele
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");

                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;

                    string filepath = Path.Combine(uploadDir, imageName);

                    FileStream fs = new FileStream(filepath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    product.Image = imageName;

                }

                _dataContext.Products.Add(product);
                await _dataContext.SaveChangesAsync();

                return RedirectToAction("ManagerProduct");
            }

            return View(product);
        }

        public async Task<IActionResult> AddQuantity(int Id)
        {
            var ProductByQuantity = await _dataContext.ProductQuantities.Where(pq => pq.ProductId == Id).ToListAsync();
            ViewBag.ProductByQuantity = ProductByQuantity;
            ViewBag.Id = Id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StoreProductQuantity(ProductQuantityModel productQuantityModel)
        {
            var product = _dataContext.Products.Find(productQuantityModel.ProductId);

            if (product == null)
            {
                return NotFound();
            }

            product.Quantity += productQuantityModel.Quantity;

            productQuantityModel.Quantity = productQuantityModel.Quantity;
            productQuantityModel.DateCreated = DateTime.Now;

            _dataContext.Add(productQuantityModel);
            _dataContext.SaveChangesAsync();
            TempData["success"] = "Thêm số lượng  sản phẩm thành công";
            return RedirectToAction("AddQuantity", "Seller", new { Id = productQuantityModel.ProductId });

        }


        public async Task<IActionResult> ManagerOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = await _dataContext.Orders.Where(o => o.UserId == userId).OrderByDescending(o => o.Id).ToListAsync();



            return View(orders);

        }

        public async Task<IActionResult> ViewOrder(string codeorder)
        {
            var DetailsOrder = await _dataContext.OrderDetails.Include(p => p.ProductModel).Where(p => p.OrderCode == codeorder).ToListAsync();
            return View(DetailsOrder);
        }



        [HttpPost]
        public async Task<IActionResult> UpdateOrders(string ordercode, int status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _dataContext.Orders.Where(o => o.UserId == userId).FirstOrDefaultAsync(o => o.OrderCode == ordercode);

            if (order == null)
            {
                return NotFound();
            }


            order.Status = status;

            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Order status updated successfully" });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { success = false, message = ex.Message });
            }

        }





    }
}
