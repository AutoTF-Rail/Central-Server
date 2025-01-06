using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Central_Server.Controllers;

[Route("/sync")]
public class SyncController : ControllerBase
{
	[HttpPost("uploadlogs")]
	public IActionResult UploadLogs()
	{
		Console.WriteLine(Request.Headers.Authorization);
		foreach (KeyValuePair<string,StringValues> keyValuePair in Request.Headers)
		{
			Console.WriteLine(keyValuePair.Key);
			Console.WriteLine(keyValuePair.Value);
		}
		return Ok();
	}
}