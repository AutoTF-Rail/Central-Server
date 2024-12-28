using Microsoft.AspNetCore.Mvc;

namespace Central_Server.Controllers;

[Route("/")]
public class RootController : ControllerBase
{
	[HttpGet("/")]
	public IActionResult Index()
	{
		return Content("Bluba");
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