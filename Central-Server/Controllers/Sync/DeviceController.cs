using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using AutoTf.Logging;
using Central_Server.Data;
using Central_Server.Extensions;
using Central_Server.Models;
using Central_Server.Models.RequestBodies;
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
	
	[HttpGet("lastsynced")]
	public IActionResult LastSynced([FromQuery, Required] string deviceName)
	{
		Guid id = _deviceDataAccess.GetUniqueId(deviceName);

		if (id == Guid.Empty)
			return NotFound("Could not find device.");
		
		DeviceStatus? status = _deviceDataAccess.GetStatus(id);
		
		// If there is no found status, the device may have just been never offline. That's why we check for it's existance above.
		if (status == null)
			return Content("");
		
		return Content(status.Timestamp.ToString("dd.MM.yyyy HH:mm:ss"));
	}
	
	[HttpGet("status")]
	public IActionResult Status([FromQuery, Required] string deviceName)
	{
		Guid id = _deviceDataAccess.GetUniqueId(deviceName);

		if (id == Guid.Empty)
			return NotFound("Could not find device.");
		
		DeviceStatus? status = _deviceDataAccess.GetStatus(id);

		// Train exists, but never started up/didn't sync yet.
		if (status == null)
			return Content("Unknown");
		
		if (DateTime.Now - status.Timestamp > TimeSpan.FromMinutes(5))
			return Content("Offline");

		return Content(status.Status);
	}
	
	[HttpGet("getAllTrains")]
	public IActionResult GetAllTrains()
	{
		return Content(JsonSerializer.Serialize(_deviceDataAccess.GetAllTrains()));
	}
	
	[HttpPost("addTrain")]
	public IActionResult AddTrain([FromBody] AddTrainBody body)
	{
		_logger.Log($"Creating new train as {body.TrainName} with authentik username {body.AuthentikUsername} and train ID {body.TrainId}.");
		
		_deviceDataAccess.CreateTrain(body.TrainName, body.AuthentikUsername, body.TrainId);
		return Ok();
	}
	
	[HttpPost("editTrain")]
	public IActionResult EditTrain([FromBody] EditTrainBody body)
	{
		_logger.Log($"Editing train {body.Id.ToString()} with new name as {body.TrainName} and new auth name {body.AuthentikUsername} and new train id {body.TrainId}.");
		
		if (_deviceDataAccess.EditTrain(body.Id, body.TrainName, body.AuthentikUsername, body.TrainId))
			return Ok();

		return NotFound();
	}
	
	[HttpPost("deleteTrain")]
	public IActionResult DeleteTrain([FromBody, Required] Guid id)
	{
		_logger.Log($"Removing train with id {id.ToString()}.");
		_deviceDataAccess.DeleteTrain(id); 
		
		return Ok();
	}

	// This has to be reported every 5 minutes, otherwise a device will be marked as offline/no signal/not found (idk yet)
	[HttpPost("updatestatus")]
	public IActionResult UpdateStatus([FromBody] string deviceStatus)
	{
		Guid id = _deviceDataAccess.GetUniqueId(Username);
		
		_deviceDataAccess.UpdateStatus(id, deviceStatus);
		return Ok();
	}
}