using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAPP.Models;
using WebAPP.Models.Repository;

namespace WebAPP.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Route("Admin/Slider")]
    [Authorize(Roles = "Admin")]
	public class SliderController : Controller
	{

        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly DataContext _dataContext;
		public SliderController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
		{
			_dataContext = dataContext;
            _webHostEnvironment= webHostEnvironment;
		}

		[Route("Index")]
		public async Task<IActionResult> Index()
		{
			return View(await _dataContext.Sliders.OrderByDescending(p => p.Id).ToListAsync());
		}


        public IActionResult Create()
        {
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderModel Slider)
        {
            

            if (ModelState.IsValid)
            {
                if (Slider.ImageUpload != null)
                {
                   
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");

                    string imageName = Guid.NewGuid().ToString() + "_" + Slider.ImageUpload.FileName;

                    string filepath = Path.Combine(uploadDir, imageName);

                    FileStream fs = new FileStream(filepath, FileMode.Create);
                    await Slider.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    Slider.Image = imageName;

                }

                _dataContext.Add(Slider);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm Slider thành công" ;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "CÓ một vài thứ đang bị lỗi";
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


            return View(Slider);
        }

        [Route("Edit")]

        public async Task<IActionResult> Edit(int Id)
        {

            SliderModel Slider = await _dataContext.Sliders.FindAsync(Id);

            return View(Slider);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SliderModel Slider)
        {

            var slider_existed=_dataContext.Sliders.Find(Slider.Id);
            if (ModelState.IsValid)
            {
                if (Slider.ImageUpload != null)
                {

                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");

                    string imageName = Guid.NewGuid().ToString() + "_" + Slider.ImageUpload.FileName;

                    string filepath = Path.Combine(uploadDir, imageName);

                    FileStream fs = new FileStream(filepath, FileMode.Create);
                    await Slider.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    slider_existed.Image = imageName;

                }
                slider_existed.Name = Slider.Name;
                slider_existed.Description = Slider.Description;
                slider_existed.Status = Slider.Status;

                _dataContext.Update(slider_existed);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật Slider thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "CÓ một vài thứ đang bị lỗi";
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


            return View(Slider);
        }

        [Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            SliderModel slider = await _dataContext.Sliders.FindAsync(Id);


           
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
                string oldfileImage = Path.Combine(uploadDir, slider.Image ?? string.Empty); // Kiểm tra null cho product.Image
                if (!string.IsNullOrEmpty(slider.Image) && System.IO.File.Exists(oldfileImage))
                {
                    System.IO.File.Delete(oldfileImage);
                }
            

                _dataContext.Sliders.Remove(slider);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Xóa slider thành công";
            
         
            return RedirectToAction("Index");
        }

    }
}
