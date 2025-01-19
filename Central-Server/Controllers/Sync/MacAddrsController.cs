using Microsoft.AspNetCore.Mvc;

namespace Central_Server.Controllers.Sync;

[ApiController]
[Route("/sync/mac")]
public class MacAddrsController : ControllerBase
{
	[HttpGet("lastmacaddrsupdate")]
	public IActionResult LastMacAddressUpdate()
	{
		return Content(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
	}

	[HttpGet("macAddress")]
	public IActionResult SyncMacAddresses()
	{
		// Get all addresses that have a timestamp of "last updated" previous to the given date from the client.
		// Keys might be deleted, but to keep sync, we won't delete themm actually, and just add a "deleted" flag to them.
		return Content("[]");
	}
}