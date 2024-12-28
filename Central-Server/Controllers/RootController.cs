using Microsoft.AspNetCore.Mvc;

namespace Central_Server.Controllers;

[Route("/meow")]
public class RootController : ControllerBase
{
	[HttpGet("meow")]
	public IActionResult Index()
	{
		return Content("Meow");
	}
}