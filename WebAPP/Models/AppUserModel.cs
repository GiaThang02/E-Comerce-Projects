using Microsoft.AspNetCore.Identity;

namespace WebAPP.Models
{
	public class AppUserModel : IdentityUser
	{
        public string FullName { get; set; }         
        public string Address { get; set; }  
        public string Gender { get; set; }           
        public int BirthYear { get; set; }           
        public string Occupation { get; set; }       
        public string RoleId { get; set; }         

    }
}
