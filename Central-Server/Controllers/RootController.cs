using Microsoft.AspNetCore.Mvc;
using FileAccess = Central_Server.Data.FileAccess;

namespace Central_Server.Controllers;

[Route("/")]
public class RootController : ControllerBase
{
	private readonly FileAccess _fileAccess;

	public RootController(FileAccess fileAccess)
	{
		_fileAccess = fileAccess;
	}
	
	/// <summary>
	/// Test endpoint
	/// </summary>
	[HttpGet("/Bluba")]
	public IActionResult Index()
	{
		return Content("Bluba");
	}
	
	[HttpGet("evuName")]
	public ActionResult<string> EvuName()
	{
		return _fileAccess.GetEvuName();
	}
	
	[HttpGet("/token")]
	public IActionResult Token()
	{
		return File("~/token.html", "text/html");
	}
	
	[HttpGet("/tokenStep")]
	public IActionResult TokenStep()
	{
		string? csrfToken = Request.Query["csrf_token"];

		KeyValuePair<string, string> proxyToken = Request.Cookies.FirstOrDefault(c => c.Key.StartsWith("authentik_proxy"));

		if (!string.IsNullOrEmpty(csrfToken) && !string.IsNullOrEmpty(proxyToken.Key))
		{
			string redirectUrl = $"http://localhost:5000/token?csrf_token={csrfToken}&proxy_name={proxyToken.Key}&proxy_token={proxyToken}";
			return Redirect(redirectUrl);
		}

		return Content("Tokens not found.");
	}
}

/// <summary>
/// Test controller
/// </summary>
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