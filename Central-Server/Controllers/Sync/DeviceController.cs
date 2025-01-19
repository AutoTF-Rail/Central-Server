using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Central_Server.Data;
using Central_Server.Models;
using Microsoft.AspNetCore.Mvc;
using FileAccess = Central_Server.Data.FileAccess;

namespace Central_Server.Controllers.Sync;

[ApiController]
[Route("/sync/device")]
public class DeviceController : ControllerBase
{
	private readonly DeviceDataAccess _deviceDataAccess;
	private readonly FileAccess _fileAccess;

	public DeviceController(DeviceDataAccess deviceDataAccess, FileAccess fileAccess)
	{
		_deviceDataAccess = deviceDataAccess;
		_fileAccess = fileAccess;
	}
	
	[HttpGet("lastsynced")]
	public IActionResult GetStatus([FromQuery, Required] string deviceName)
	{
		Console.WriteLine($"Getting last synced date for: {deviceName}.");
		DeviceStatus? status = _deviceDataAccess.GetStatusByName(deviceName);
		
		if (status == null)
			return BadRequest("Device not found.");
		
		return Content(status.Timestamp.ToString("dd.MM.yyyy HH:mm:ss"));
	}
	
	[HttpGet("status")]
	public IActionResult Status([FromQuery, Required] string deviceName)
	{
		Console.WriteLine($"Getting status for: {deviceName}.");
		DeviceStatus? status = _deviceDataAccess.GetStatusByName(deviceName);
		
		if (status == null)
			return BadRequest("Device not found.");
		
		if ((DateTime.Now - status.Timestamp) > TimeSpan.FromMinutes(5))
			return Content("Offline");

		return Content(status.Status);
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
	public IActionResult GetLogs([FromQuery, Required] string deviceName, [FromQuery, Required] string date)
	{
		try
		{
			Console.WriteLine($"Logs requested for: {deviceName} at {date}.");

			string dir = Path.Combine("Logs", deviceName, date + ".txt");
			
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
	public IActionResult IndexLogs([FromQuery, Required] string deviceName)
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
}