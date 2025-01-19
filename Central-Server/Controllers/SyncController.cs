using Central_Server.Data;
using Microsoft.AspNetCore.Mvc;
using FileAccess = Central_Server.Data.FileAccess;

namespace Central_Server.Controllers;

[ApiController]
[Route("/sync")]
public class SyncController : ControllerBase
{
	private readonly FileAccess _fileAccess;
	private readonly DeviceDataAccess _deviceDataAccess;

	public SyncController(FileAccess fileAccess, DeviceDataAccess deviceDataAccess)
	{
		_fileAccess = fileAccess;
		_deviceDataAccess = deviceDataAccess;
	}
}