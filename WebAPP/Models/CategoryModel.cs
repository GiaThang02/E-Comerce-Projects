using System.ComponentModel.DataAnnotations;

namespace WebAPP.Models
{
	public class CategoryModel
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage ="Yêu cầu nhập danh mục")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Yêu cầu mô tả danh mục")]
		public string Description { get; set; }
	
		public string Slug { get; set; }
		
		public int Status { get; set; }

	}
}
