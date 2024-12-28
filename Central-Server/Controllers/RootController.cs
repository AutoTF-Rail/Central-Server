using Microsoft.AspNetCore.Mvc;

namespace Central_Server.Controllers;

[Route("/meow")]
public class RootController : ControllerBase
{
	private readonly FileAccess _fileAccess;

	public RootController(FileAccess fileAccess)
	{
		_fileAccess = fileAccess;
	}
	
	[HttpGet("meow")]
	public IActionResult Index()
	{
		if (!_fileAccess.ReadFile("meow", out string content))
		{
			_fileAccess.WriteAllText("meow", "meow meow");
			return Content("Meow didn't exist");
		}
		else
		{
			return Content("Meow contained: " + content);
		}
	}
}