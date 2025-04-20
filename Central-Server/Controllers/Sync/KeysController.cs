using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Central_Server.Data;
using Central_Server.Models.RequestBodies;
using Microsoft.AspNetCore.Mvc;

namespace Central_Server.Controllers.Sync;

[ApiController]
[Route("/sync/keys")]
public class KeysController : ControllerBase
{
	private readonly KeyDataAccess _keyDataAccess;

	public KeysController(KeyDataAccess keyDataAccess)
	{
		_keyDataAccess = keyDataAccess;
	}
	
	[HttpGet("validate")]
	public IActionResult Validate([FromQuery, Required] string serialNumber, [FromQuery, Required] string code, [FromQuery, Required] DateTime timestamp)
	{
		try
		{
			if (!_keyDataAccess.CheckForKey(serialNumber))
				return NotFound();
			
			if (!_keyDataAccess.Validate(serialNumber, code, timestamp))
				return Unauthorized();

			_keyDataAccess.UpdateVerified(serialNumber, true);
			return Ok();
		}
		catch
		{
			return BadRequest();
		}
	}
	
	[HttpGet("lastupdate")]
	public IActionResult LastKeysUpdate()
	{
		return Content(_keyDataAccess.GetLastChanged().ToString("dd.MM.yyyy HH:mm:ss"));
	}

	[HttpGet("new")]
	public IActionResult SyncKeys([FromQuery, Required] string lastSynced)
	{
		try
		{
			return Content(JsonSerializer.Serialize(_keyDataAccess.GetNew(DateTime.Parse(lastSynced))));
		}
		catch
		{
			return BadRequest();
		}
	}

	[HttpGet("all")]
	public IActionResult SyncAllKeys()
	{
		try
		{
			return Content(JsonSerializer.Serialize(_keyDataAccess.GetAll()));
		}
		catch
		{
			return BadRequest();
		}
	}

	[HttpPost("add")]
	public IActionResult AddKey([FromBody] AddKeyBody body)
	{
		try
		{
			_keyDataAccess.CreateKey(body.SerialNumber, body.Secret);
			return Ok();
		}
		catch
		{
			return BadRequest();
		}
	}
}