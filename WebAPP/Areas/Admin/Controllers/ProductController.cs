using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAPP.Models;
using WebAPP.Models.Repository;


namespace WebAPP.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
	{

		private readonly DataContext _dataContext;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)	
		{
			_dataContext = context;
			_webHostEnvironment = webHostEnvironment;
		}
        public async Task<IActionResult> Index(string searchString)
        {
            var products = from p in _dataContext.Products
                           .OrderByDescending(p => p.Id)
                           .Include(p => p.Category)
                           .Include(p => p.Brand)
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.Contains(searchString) || s.Description.Contains(searchString));
            }

            return View(await products.ToListAsync());
        }

        [HttpGet]
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
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name",product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name",product.BrandId);

			if (ModelState.IsValid)
			{
				product.Slug = product.Name.Replace(" ","-");
				var slug= await _dataContext.Products.FirstOrDefaultAsync(p=>p.Slug ==product.Slug);
				if(slug != null)
				{
					ModelState.AddModelError("", "Sản phẩm đã có trong database");
					return View(product);
				}
				
				
			if(product.ImageUpload != null)
				{
					//detele
						string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");

                        string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;

						string filepath= Path.Combine(uploadDir,imageName);

						FileStream fs= new FileStream(filepath,FileMode.Create);
						await product.ImageUpload.CopyToAsync(fs);
						fs.Close();
						product.Image = imageName;

				}


				
				_dataContext.Add(product);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Thêm sản phẩm thành công";
				return RedirectToAction("Index");
			}
			else
			{
				TempData["error"] = "CÓ một vài thứ đang bị lỗi !";
				List<String> errors = new List<String>();
				foreach(var value in ModelState.Values)
				{

					foreach(var error in value.Errors) 
					{
						errors.Add(error.ErrorMessage);
					}

				}
                string errorMessage = string.Join("\n", errors);
				return BadRequest(errorMessage);
            }


            return View(product);
        }
        public async Task<IActionResult> Edit(int Id)
        {

            ProductModel product = await _dataContext.Products.FindAsync(Id);
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);



            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id,ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
			var existed_product=_dataContext.Products.Find(product.Id); // tìm sản phầm theo Id product


            if (ModelState.IsValid)
            {
                product.Slug = product.Name.Replace(" ", "-");
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
              


				if (product.ImageUpload != null)
				{

					//update anh moi
					string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");

					string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;

					string filepath = Path.Combine(uploadDir, imageName);

					//xoa anh cu
					string oldfileImage = Path.Combine(uploadDir, existed_product.Image);
					try
					{
						if (System.IO.File.Exists(oldfileImage))
						{
							System.IO.File.Delete(oldfileImage);

						}
					}
					catch (Exception ex)
					{
						ModelState.AddModelError("", "Bi loi");
					}


					FileStream fs = new FileStream(filepath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    existed_product.Image = imageName;


                   
                   
                }

				existed_product.Name= product.Name;
				existed_product.Description= product.Description;	
				existed_product.Price= product.Price;
				existed_product.Category= product.Category;
				existed_product.Brand= product.Brand;



                _dataContext.Update(existed_product);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật sản phẩm thành công";
                return RedirectToAction("Index");
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
                return BadRequest(errorMessage);
            }


            return View(product);
        }

		public async Task<IActionResult> Delete(int Id)
		{
			ProductModel product = await _dataContext.Products.FindAsync(Id);
			if (product != null && !string.Equals(product.Image, "noname.jpg"))
			{
				string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
				string oldfileImage = Path.Combine(uploadDir, product.Image ?? string.Empty); // Kiểm tra null cho product.Image
				if (!string.IsNullOrEmpty(product.Image) && System.IO.File.Exists(oldfileImage))
				{
					System.IO.File.Delete(oldfileImage);
				}
			}
			if (product != null)
			{
				_dataContext.Products.Remove(product);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Xóa sản phẩm thành công";
			}
			else
			{
				TempData["error"] = "Không tìm thấy sản phẩm";
			}
			return RedirectToAction("Index");
		}

		[Route("AddQuantity")]
        public async Task<IActionResult> AddQuantity(int Id)
        {
			var ProductByQuantity = await _dataContext.ProductQuantities.Where(pq => pq.ProductId == Id).ToListAsync();
			ViewBag.ProductByQuantity = ProductByQuantity;
            ViewBag.Id= Id;
            return View();
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StoreProductQuantity(ProductQuantityModel productQuantityModel)
		{
			var product=_dataContext.Products.Find(productQuantityModel.ProductId);

			if(product == null)
			{
				return NotFound();
			}

			product.Quantity += productQuantityModel.Quantity;

            productQuantityModel.Quantity = productQuantityModel.Quantity;
			productQuantityModel.DateCreated = DateTime.Now;

			_dataContext.Add(productQuantityModel);
			_dataContext.SaveChangesAsync();
			TempData["success"] = "Thêm số lượng  sản phẩm thành công";
			return RedirectToAction("AddQuantity","Product", new {Id = productQuantityModel.ProductId});

        }


    }
}
