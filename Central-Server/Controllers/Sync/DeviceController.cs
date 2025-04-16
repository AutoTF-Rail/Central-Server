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
	
	// TODO: Rename endpoint?
	[HttpGet("lastsynced")]
	public IActionResult LastSynced([FromQuery, Required] string deviceName)
	{
		_logger.Log($"Getting last synced date for {deviceName}.");
		
		DeviceStatus? status = _deviceDataAccess.GetStatusByName(deviceName);
		
		if (status == null)
			return BadRequest("Device not found.");
		
		return Content(status.Timestamp.ToString("dd.MM.yyyy HH:mm:ss"));
	}
	
	[HttpGet("status")]
	public IActionResult Status([FromQuery, Required] string deviceName)
	{
		_logger.Log($"Getting status for {deviceName}.");
		
		DeviceStatus? status = _deviceDataAccess.GetStatusByName(deviceName);
		
		if (status == null)
			return BadRequest("Device not found.");
		
		if (DateTime.Now - status.Timestamp > TimeSpan.FromMinutes(5))
			return Content("Offline");

		return Content(status.Status);
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
	
	[HttpGet("getAllTrains")]
	public IActionResult GetAllTrains()
	{
		return Content(JsonSerializer.Serialize(_deviceDataAccess.GetAllTrains()));
	}
	
	[HttpPost("addTrain")]
	public IActionResult AddTrain([FromQuery, Required] string trainName, [FromQuery, Required] string authentikUsername, [FromQuery, Required] string trainId)
	{
		_logger.Log($"Creating new train as {trainName} with authentik username {authentikUsername} and train ID {trainId}.");
		
		_deviceDataAccess.CreateTrain(trainName, authentikUsername, trainId);
		return Ok();
	}
	
	[HttpPost("editTrain")]
	public IActionResult EditTrain([FromQuery, Required] Guid id, [FromQuery, Required] string newTrainName, [FromQuery, Required] string newAuthUsername, [FromQuery, Required] string newTrainId)
	{
		_logger.Log($"Editing train {id.ToString()} with new name as {newTrainName} and new auth name {newAuthUsername} and new train id {newTrainId}.");
		
		if (_deviceDataAccess.EditTrain(id, newTrainName, newAuthUsername, newTrainId))
			return Ok();

		return NotFound();
	}
	
	[HttpPost("deleteTrain")]
	public IActionResult DeleteTrain([FromQuery, Required] Guid id)
	{
		_logger.Log($"Removing train with id {id.ToString()}.");
		_deviceDataAccess.DeleteTrain(id);
		return Ok();
	}

	// This has to be reported every 5 minutes, otherwise a device will be marked as offline/no signal/not found (idk yet)
	[HttpPost("updatestatus")]
	public IActionResult UpdateStatus([FromBody]string deviceStatus)
	{
		_logger.Log($"Updating status for {Username} as {deviceStatus}.");
		
		_deviceDataAccess.UpdateStatus(Username, deviceStatus);
		return Ok();
	}
}