using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using AutoTf.Logging;
using Central_Server.Data;
using Microsoft.AspNetCore.Mvc;

namespace Central_Server.Controllers.Sync;

[ApiController]
[Route("/sync/mac")]
public class MacAddrsController : ControllerBase
{
	private readonly MacAddrAccess _macAddrAccess;
	private readonly Logger _logger;

	public MacAddrsController(MacAddrAccess macAddrAccess, Logger logger)
	{
		_macAddrAccess = macAddrAccess;
		_logger = logger;
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
	public IActionResult AddAddress([FromBody, Required] string address)
	{
		try
		{
			_logger.Log($"Adding Address {address}.");
			
			_macAddrAccess.CreateAddress(address);
			return Ok();
		}
		catch (Exception e)
		{
			_logger.Log("Error while adding a mac address:");
			_logger.Log(e.ToString());
		}

		return BadRequest("An error occured while adding the mac address.");
	}
}