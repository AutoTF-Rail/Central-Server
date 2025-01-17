using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Central_Server.Data;
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
	
	[HttpGet("lastkeysupdate")]
	public IActionResult LastKeysUpdate()
	{
		return Content(_keyDataAccess.GetLastChanged().ToString("dd.MM.yyyy HH:mm:ss"));
	}

	[HttpGet("newkeys")]
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

	[HttpGet("allkeys")]
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

	[HttpPost("addkey")]
	public IActionResult SyncKeys([FromQuery, Required] string serialNumber, [FromQuery, Required] string secret)
	{
		try
		{
			_keyDataAccess.CreateKey(serialNumber, secret);
			return Ok();
		}
		catch
		{
			return BadRequest();
		}
	}
}