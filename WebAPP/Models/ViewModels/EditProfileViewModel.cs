using System.ComponentModel.DataAnnotations;

namespace WebAPP.Models.ViewModels
{
    public class EditProfileViewModel
    {
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

       
    }
}
