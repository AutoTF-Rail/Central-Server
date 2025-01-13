using Microsoft.AspNetCore.Mvc;
using FileAccess = Central_Server.Data.FileAccess;

namespace Central_Server.Controllers;

[Route("/")]
public class RootController : ControllerBase
{
	[HttpGet("/Bluba")]
	public IActionResult Index()
	{
		return Content("Bluba");
	}
	
	[HttpGet("/token")]
	public IActionResult Token()
	{
		return File("~/token.js", "application/javascript");
	}
}
[Route("/meow")]
public class MeowController : ControllerBase
{
	private readonly FileAccess _fileAccess;

	public MeowController(FileAccess fileAccess)
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