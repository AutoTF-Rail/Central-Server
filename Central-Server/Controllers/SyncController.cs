using Central_Server.Data;
using Microsoft.AspNetCore.Mvc;
using FileAccess = Central_Server.Data.FileAccess;

namespace Central_Server.Controllers;

[ApiController]
[Route("/sync")]
public class SyncController : ControllerBase
{
	private readonly FileAccess _fileAccess;
	private readonly DeviceDataAccess _deviceDataAccess;

	public SyncController(FileAccess fileAccess, DeviceDataAccess deviceDataAccess)
	{
		_fileAccess = fileAccess;
		_deviceDataAccess = deviceDataAccess;
	}

	// This has to be reported every 5 minutes, otherwise a device will be marked as offline/no signal/not found (idk yet)
	// deviceInfo: 
	// 1: status
	[HttpPost("updatestatus")]
	public IActionResult UpdateStatus([FromBody]string deviceStatus)
	{
		string? device = Request.Headers["X-Authentik-Username"];
			
		if (device == null)
			return BadRequest("X-Authentik-Username was null");
		
		Console.WriteLine($"Updating status for: {device} as {deviceStatus}.");
		_deviceDataAccess.UpdateStatus(device, deviceStatus);

		return Ok();
	}

	[HttpGet("lastmacaddrsupdate")]
	public IActionResult LastMacAddressUpdate()
	{
		return Content(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
	}

	[HttpGet("macAddress")]
	public IActionResult SyncMacAddresses()
	{
		// Get all addresses that have a timestamp of "last updated" previous to the given date from the client.
		// Keys might be deleted, but to keep sync, we won't delete themm actually, and just add a "deleted" flag to them.
		return Content("[]");
	}
}