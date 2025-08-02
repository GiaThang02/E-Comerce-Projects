using Microsoft.EntityFrameworkCore;
namespace WebAPP.Models.Repository
{
	public class SeedData
	{
		public static void SeedingData(DataContext _context)
		{
			_context.Database.Migrate();
			if (!_context.Products.Any()) //chuaw co du lieu
			{
				CategoryModel macbook = new CategoryModel { Name = "macbook", Slug = "macbook", Description = "macbook ", Status = 1 };
				CategoryModel pc = new CategoryModel { Name = "pc", Slug = "pc", Description = "pcasd", Status = 1 };


				BrandModel apple = new BrandModel { Name = "Apple", Slug = "Apple", Description = "Apple is dt", Status = 1 };
				BrandModel samsung = new BrandModel { Name = "Samsung", Slug = "samsung", Description = "Samsung is dt", Status = 1 };


				_context.Products.AddRange(

					new ProductModel { Name = "Macbook", Slug = "macbook", Description = "Macbook is 1", Image = "1.jpg", Category = macbook, Brand= apple, Price=1232 },
					new ProductModel { Name = "PC", Slug = "pc", Description = "pc is 2", Image = "1.jpg", Category = pc, Brand= samsung, Price=12324 }
				);
				_context.SaveChanges();
			}
		}
	}
}
