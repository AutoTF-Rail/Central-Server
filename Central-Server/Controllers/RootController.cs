using Microsoft.AspNetCore.Mvc;

namespace Central_Server.Controllers;

[Route("/")]
public class RootController : ControllerBase
{
	[HttpGet("/")]
	public IActionResult Index()
	{
		return Content("Meow");
	}
}