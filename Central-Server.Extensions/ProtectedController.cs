using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Central_Server.Extensions;

[ApiController]
public class ProtectedController : ControllerBase
{
	protected string Username { get; private set; } = null!;

	public ProtectedController()
	{
		IHeaderDictionary? headers = HttpContext.Request.Headers;

		if (!IsAllowedDevice(headers, out string? deviceName))
		{
			Response.StatusCode = StatusCodes.Status401Unauthorized;
		}
		else
			Username = deviceName!;
	}
	
	private static bool IsAllowedDevice(IHeaderDictionary headers, out string? deviceName)
	{
		deviceName = "debugPlaceholder";
            
		try
		{
#if DEBUG
			return true;
#endif
			deviceName = headers["X-Authentik-Username"].ToString();
                
			// Validate the username (you might want to add additional logic here)
			return !string.IsNullOrEmpty(deviceName);
		}
		catch
		{
			return false;
		}
	}
}