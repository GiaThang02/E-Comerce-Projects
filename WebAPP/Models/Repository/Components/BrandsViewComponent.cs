using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPP.Models.Repository.Components
{
	public class BrandsViewComponent : ViewComponent
	{

		private readonly DataContext _dataContext;
		public BrandsViewComponent(DataContext context)
		{
			_dataContext = context;
		}

		public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Brands.ToListAsync());
		//chỗ này mới kiểu lấy được dữ liệu từ database th chứ chưa đồng bộ  với shared => tạo newfolder gọi là component trong shared
		//Tạo file component trong shared thì tên tiền tố như categories nói chung phải là tiền tố còn ViewComponent là hậu tố *
	}
}
