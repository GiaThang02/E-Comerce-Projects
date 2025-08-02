using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPP.Models.Repository.Vadidation;
namespace WebAPP.Models
{
	public class ProductModel
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Yêu cầu nhập tên sản phẩm")]
		public string Name { get; set; }
		public string Slug { get; set; }
		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả sản phẩm ")]
		public string Description { get; set; }
		[Required( ErrorMessage = "Yêu cầu nhập giá sản phẩm")]
		[Range(0.01,double.MaxValue)]
		[Column(TypeName ="decimal(8,2)")]
		public decimal Price { get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Yêu cầu chon thương hiệu")]
        public int BrandId {  get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Yêu cầu chon danh mục")]
        public int CategoryId { get; set; }

        public int Quantity { get; set; }
        public int Sold { get; set; }
        public CategoryModel Category { get; set; }
		public BrandModel Brand { get; set; }
		public string Image { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUserModel? User { get; set; } 

        public RatingModel Ratings { get; set; }
        [NotMapped]
		[FileExtension]
		public IFormFile? ImageUpload { get; set;} //? co cung duocjd không có cũng chẳng sao

	}
}
