using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPP.Models
{
	public class RatingModel
	{
		[Key]
		public int Id { get; set; }

		public int ProductId { get; set; }

		public string Name { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập nội dung")]

		public string Comment { get; set; }
		public int Star { get; set; }

		[Required(ErrorMessage = "Yêu cầu nhập email")]

		public string Email {  get; set; }

		[ForeignKey("ProductId")]
		public ProductModel  product { get; set;}


	}
}
