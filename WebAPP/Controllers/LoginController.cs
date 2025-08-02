using Microsoft.AspNetCore.Mvc;

namespace WebAPP.Controllers
{
	public class LoginController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
