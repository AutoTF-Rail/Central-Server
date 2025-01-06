using System.Text.Json;
using Central_Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace Central_Server.Controllers;

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
	// 1: device username
	// 2: status
	[HttpPost("updatestatus")]
	public IActionResult UpdateStatus([FromBody]string[] deviceInfo)
	{
		if (deviceInfo.Length != 2)
			return BadRequest("Please supply proper device information.");
		
		Console.WriteLine($"Updating status for: {deviceInfo[0]} as {deviceInfo[1]}.");
		_deviceDataAccess.UpdateStatus(deviceInfo[0], deviceInfo[1]);
	}
	
	[HttpGet("getstatus")]
	public IActionResult GetStatus([FromBody]string deviceName)
	{
		Console.WriteLine($"Getting status for: {deviceName}.");
		DeviceStatus? status = _deviceDataAccess.GetStatusByName(deviceName);
		
		if (status == null)
			return BadRequest("Device not found.");
		
		if ((DateTime.Now - status.Timestamp) > TimeSpan.FromMinutes(5))
			return Content("Offline");

		return Content(status.Status);
	}

	[HttpGet("macAddress")]
	public IActionResult SyncMacAddresses()
	{
		return Content("");
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
	
	// deviceInfo:
	// 1: device username
	// 2: date (yyyy-mm-dd)
	[HttpGet("getlogs")]
	public IActionResult GetLogs([FromBody] string[] deviceInfo)
	{
		try
		{
			if (deviceInfo.Length != 2)
				return BadRequest("Please supply proper device information.");
			
			Console.WriteLine($"Logs requested for: {deviceInfo[0]} at {deviceInfo[1]}.");

			string dir = Path.Combine("Logs", deviceInfo[0], deviceInfo[1] + ".txt");
			
			if (!_fileAccess.FileExists(dir))
				return NotFound("Could not find log file.");

			return Content(JsonSerializer.Serialize(_fileAccess.ReadAllLines(dir)));
		}
		catch (Exception e)
		{
			Console.WriteLine("Could not provide logs:");
			Console.WriteLine(e.Message);
		}

		return BadRequest("Could not supply logs.");
	}

	[HttpGet("indexlogs")]
	public IActionResult IndexLogs([FromBody] string deviceName)
	{
		try
		{
			Console.WriteLine($"Logs index requested for: {deviceName}.");

			string dir = Path.Combine("Logs", deviceName);
			if (!_fileAccess.DirectoryExists(dir))
				return NotFound("Could not find device.");

			string[] files = _fileAccess.GetFiles(dir);

			return Content(JsonSerializer.Serialize(files.Select(Path.GetFileNameWithoutExtension)));
		}
		catch (Exception e)
		{
			Console.WriteLine("Could not provide log index.");
			Console.WriteLine(e.Message);
		}

		return BadRequest("Could not supply log index.");
	}
}