using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPP.Models.Repository.Vadidation;

namespace WebAPP.Models
{
	public class SliderModel
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập tên slider ")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập ")]
		public string Description { get; set; }

		public int? Status { get; set; }
		public string Image { get; set; }
		[NotMapped]
		[FileExtension]
		public IFormFile? ImageUpload { get; set; } //? co cung duocjd không có cũng chẳng sao

	}
}
