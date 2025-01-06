using Microsoft.AspNetCore.Mvc;

namespace Central_Server.Controllers;

[Route("/sync")]
public class SyncController : ControllerBase
{
	private readonly FileAccess _fileAccess;

	public SyncController(FileAccess fileAccess)
	{
		_fileAccess = fileAccess;
	}
	
	[HttpPost("uploadlogs")]
	public IActionResult UploadLogs([FromBody]string[] logs)
	{
		try
		{
			string? device = Request.Headers["X-Authentik-Username"];
			
			if (device == null)
				return BadRequest("X-Authentik-Username was null");
			if (logs.Length == 0)
				return BadRequest("Please supply logs in the body to upload");
			
			
			Console.WriteLine($"SYNC: {device} requested to upload logs.");
			_fileAccess.AppendAllLines(Path.Combine("Logs", device, DateTime.Now.ToString("yyyy-MM-dd") + ".txt"),
				logs);
			
			Console.WriteLine($"SYNC: Successfully uploaded logs for {device}.");
			return Ok();
		}
		catch (Exception e)
		{
			Console.WriteLine("SYNC: ERROR: Could not upload logs:");
			Console.WriteLine(e.Message);
		}

		return BadRequest();
	}
}