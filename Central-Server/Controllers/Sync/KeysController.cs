using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Central_Server.Data;
using Central_Server.Models;
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
	public ActionResult<List<KeyData>> SyncKeys([FromQuery, Required] string lastSynced)
	{
		try
		{
			return _keyDataAccess.GetNew(DateTime.Parse(lastSynced));
		}
		catch
		{
			return BadRequest();
		}
	}

	[HttpGet("all")]
	public ActionResult<List<KeyData>> SyncAllKeys()
	{
		try
		{
			return _keyDataAccess.GetAll();
		}
		catch (Exception ex)
		{
			Console.WriteLine("An error occured while getting all keys:");
			Console.WriteLine(ex.ToString());
			return BadRequest("An error occured while getting all keys.");
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