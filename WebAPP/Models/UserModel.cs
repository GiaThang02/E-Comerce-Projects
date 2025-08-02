using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPP.Models
{
	public class UserModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage="Vui lòng nhập UserName")]	
		
		public string UserName { get; set; }
		[Required(ErrorMessage = "Nhập UserName"), EmailAddress]
		public string Email { get; set; }

		[DataType(DataType.Password),Required(ErrorMessage ="Vui lòng nhập mật khẩu")]
		public string Password { get; set; }

        [Display(Name = "Họ và Tên")]
        public string FullName { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số Điện Thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Giới Tính")]
        public string Gender { get; set; }


        [Display(Name = "Năm Sinh")]
        public int BirthYear { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]

        public AppUserModel User { get; set; }


    }
}
