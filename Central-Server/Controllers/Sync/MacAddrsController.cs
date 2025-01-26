using System.Text.Json;
using Central_Server.Data;
using Microsoft.AspNetCore.Mvc;

namespace Central_Server.Controllers.Sync;

[ApiController]
[Route("/sync/mac")]
public class MacAddrsController : ControllerBase
{
	private readonly MacAddrAccess _macAddrAccess;

	public MacAddrsController(MacAddrAccess macAddrAccess)
	{
		_macAddrAccess = macAddrAccess;
	}
	
	
	[HttpGet("lastmacaddrsupdate")]
	public IActionResult LastMacAddressUpdate()
	{
		return Content(_macAddrAccess.GetLastChanged().ToString("dd.MM.yyyy HH:mm:ss"));
	}

	[HttpGet("macAddress")]
	public IActionResult SyncMacAddresses()
	{
		// If last changed value is never than what the client has, we send all back
		return Content(JsonSerializer.Serialize(_macAddrAccess.GetAll()));
	}

	[HttpPost("addAddress")]
	public IActionResult AddAddress([FromBody] string address)
	{
		try
		{
			if (string.IsNullOrEmpty(address))
			{
				Console.WriteLine("Could not add address due to it being null or empty: " + address);
				return NotFound();
			}

			Console.WriteLine("Adding Address: " + address);
			_macAddrAccess.CreateAddress(address);
			return Ok();
		}
		catch (Exception e)
		{
			Console.WriteLine("Error while adding a mac address:");
			Console.WriteLine(e);
		}

		return BadRequest();
	}
}