using Microsoft.AspNetCore.Mvc;

namespace Central_Server.Controllers;

[Route("/sync")]
public class SyncController : ControllerBase
{
	[HttpPost("uploadlogs")]
	public IActionResult UploadLogs()
	{
		try
		{
			Console.WriteLine(Request.Headers["X-Authentik-Username"] + " requested to upload logs.");
		
			return Ok();
		}
		catch (Exception e)
		{
			Console.WriteLine("ERROR: Could not upload logs:");
			Console.WriteLine(e.Message);
		}

		return BadRequest();
	}
}