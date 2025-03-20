using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using AutoTf.Logging;
using Central_Server.Data;
using Central_Server.Extensions;
using Central_Server.Models;
using Microsoft.AspNetCore.Mvc;
using FileAccess = Central_Server.Data.FileAccess;

namespace Central_Server.Controllers.Sync;

[ApiController]
[Route("/sync/device")]
public class DeviceController : AuthentikController
{
	private readonly DeviceDataAccess _deviceDataAccess;
	private readonly FileAccess _fileAccess;
	private readonly Logger _logger;

	public DeviceController(DeviceDataAccess deviceDataAccess, FileAccess fileAccess, Logger logger)
	{
		_deviceDataAccess = deviceDataAccess;
		_fileAccess = fileAccess;
		_logger = logger;
	}
	
	[HttpGet("getvideo")]
	public IActionResult GetVideo([FromQuery, Required] string deviceName, [FromQuery, Required] string date)
	{
		try
		{
			_logger.Log($"Video requested for: {deviceName} at {date}.");

			string dir = Path.Combine("Videos", deviceName, date + ".mp4");
			
			if (!_fileAccess.FileExists(dir))
				return NotFound("Could not find video file.");

			return File(_fileAccess.ReadAllBytes(dir), "video/mp4", date + ".mp4");
		}
		catch (Exception e)
		{
			_logger.Log("Could not provide video:");
			_logger.Log(e.ToString());
		}

		return BadRequest("Could not supply video.");
	}
	
	[HttpGet("indexvideos")]
	public IActionResult IndexVideos([FromQuery, Required] string deviceName)
	{
		try
		{
			_logger.Log($"Video index requested for: {deviceName}.");

			string dir = Path.Combine("Videos", deviceName);
			
			if (!_fileAccess.DirectoryExists(dir))
				return NotFound("Could not find device.");

			string[] files = _fileAccess.GetFiles(dir);

			return Content(JsonSerializer.Serialize(files.Select(Path.GetFileNameWithoutExtension)));
		}
		catch (Exception e)
		{
			_logger.Log("Could not provide video index.");
			_logger.Log(e.ToString());
		}

		return BadRequest("Could not supply video index.");
	}
	
	[HttpPost("uploadvideo")]
	public IActionResult UploadLogs([FromForm] IFormFile file)
	{
		try
		{
			if (file.Length == 0)
				return BadRequest("Please supply a video in the body to upload");
			
			try
			{
				_logger.Log($"SYNC: {Username} requested to upload video \"{file.FileName}\".");
				
				_fileAccess.SaveVideo(Path.Combine("Videos", Username, file.FileName), file);
				
				_logger.Log($"SYNC: Successfully uploaded video \"{file.FileName}\".");
				return Ok();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
		catch (Exception e)
		{
			_logger.Log("SYNC: ERROR: Could not upload video:");
			_logger.Log(e.ToString());
		}

		return BadRequest();
	}
	
	// TODO: Rename endpoint?
	[HttpGet("lastsynced")]
	public IActionResult LastSynced([FromQuery, Required] string deviceName)
	{
		_logger.Log($"Getting last synced date for: {deviceName}.");
		DeviceStatus? status = _deviceDataAccess.GetStatusByName(deviceName);
		
		if (status == null)
			return BadRequest("Device not found.");
		
		return Content(status.Timestamp.ToString("dd.MM.yyyy HH:mm:ss"));
	}
	
	[HttpGet("status")]
	public IActionResult Status([FromQuery, Required] string deviceName)
	{
		_logger.Log($"Getting status for: {deviceName}.");
		DeviceStatus? status = _deviceDataAccess.GetStatusByName(deviceName);
		
		if (status == null)
			return BadRequest("Device not found.");
		
		if ((DateTime.Now - status.Timestamp) > TimeSpan.FromMinutes(5))
			return Content("Offline");

		return Content(status.Status);
	}
	
	[HttpPost("uploadlogs")]
	public IActionResult UploadLogs([FromBody] string[] logs)
	{
		try
		{
			if (logs.Length == 0)
				return BadRequest("Please supply logs in the body to upload");
			
			
			_logger.Log($"SYNC: {Username} requested to upload logs.");
			_fileAccess.AppendAllLines(Path.Combine("Logs", Username, DateTime.Now.ToString("yyyy-MM-dd") + ".txt"),
				logs);
			
			_logger.Log($"SYNC: Successfully uploaded logs for {Username}.");
			return Ok();
		}
		catch (Exception e)
		{
			_logger.Log("SYNC: ERROR: Could not upload logs:");
			_logger.Log(e.ToString());
		}

		return BadRequest();
	}
	
	[HttpGet("getlogs")]
	public IActionResult GetLogs([FromQuery, Required] string deviceName, [FromQuery, Required] string date)
	{
		try
		{
			_logger.Log($"Logs requested for: {deviceName} at {date}.");

			string dir = Path.Combine("Logs", deviceName, date + ".txt");
			
			if (!_fileAccess.FileExists(dir))
				return NotFound("Could not find log file.");

			return Content(JsonSerializer.Serialize(_fileAccess.ReadAllLines(dir)));
		}
		catch (Exception e)
		{
			_logger.Log("Could not provide logs:");
			_logger.Log(e.ToString());
		}

		return BadRequest("Could not supply logs.");
	}

	[HttpGet("indexlogs")]
	public IActionResult IndexLogs([FromQuery, Required] string deviceName)
	{
		try
		{
			_logger.Log($"Logs index requested for: {deviceName}.");

			string dir = Path.Combine("Logs", deviceName);
			
			if (!_fileAccess.DirectoryExists(dir))
				return NotFound("Could not find device.");

			string[] files = _fileAccess.GetFiles(dir);

			return Content(JsonSerializer.Serialize(files.Select(Path.GetFileNameWithoutExtension)));
		}
		catch (Exception e)
		{
			_logger.Log("Could not provide log index.");
			_logger.Log(e.ToString());
		}

		return BadRequest("Could not supply log index.");
	}

	// TODO: Remove?
	[HttpGet("devices")]
	public IActionResult Devices()
	{
		// TODO: Implement BETTER Device index. (db or via authentik?) 
		// TODO: TryCatch
		string[] directories = _fileAccess.GetDirectories("Logs");
		return Content(JsonSerializer.Serialize(directories));
	}
	
	// TODO: Docs
	[HttpPost("addTrain")]
	public IActionResult AddTrain([FromQuery, Required] string trainName, [FromQuery, Required] string authentikUsername, [FromQuery, Required] string trainId)
	{
		_logger.Log($"Creating new train as {trainName} with authentik username {authentikUsername} and train ID {trainId}.");
		_deviceDataAccess.CreateTrain(trainName, authentikUsername, trainId);
		return Ok();
	}
	
	// TODO: Docs
	[HttpGet("getAllTrains")]
	public IActionResult GetAllTrains()
	{
		return Content(JsonSerializer.Serialize(_deviceDataAccess.GetAllTrains()));
	}

	// This has to be reported every 5 minutes, otherwise a device will be marked as offline/no signal/not found (idk yet)
	[HttpPost("updatestatus")]
	public IActionResult UpdateStatus([FromBody]string deviceStatus)
	{
		_logger.Log($"Updating status for: {Username} as {deviceStatus}.");
		_deviceDataAccess.UpdateStatus(Username, deviceStatus);

		return Ok();
	}
}