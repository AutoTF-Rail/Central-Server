using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Central_Server.Extensions;

public class AuthentikController : ControllerBase, IActionFilter
{
	protected string Username { get; private set; } = null!;
	
	public void OnActionExecuting(ActionExecutingContext context)
	{
		IHeaderDictionary? headers = context.HttpContext.Request.Headers;

		if (!IsAllowedDevice(context.HttpContext, headers, out string? deviceName))
		{
			context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
			context.Result = new UnauthorizedResult();
		}
		else
			Username = deviceName!;
	}

	public void OnActionExecuted(ActionExecutedContext context) { }
	
	private static bool IsAllowedDevice(HttpContext context, IHeaderDictionary headers, out string? deviceName)
	{
		deviceName = "system";
            
		try
		{
#if DEBUG
			return true;
#endif
			// ReSharper disable once HeuristicUnreachableCode
#pragma warning disable CS0162 // Unreachable code detected
			
			string? remoteIp = context.Connection.RemoteIpAddress?.ToString();
			
			if (remoteIp == null)
				return false;
			
			if (remoteIp == "127.0.0.1" || remoteIp == "::1" || remoteIp.StartsWith("172.17.0.1"))
				return true;
			
			deviceName = headers["X-Authentik-Username"].ToString();
                
			// We don't need to further validate this, because all incoming traffic is being routed through authentik anyways, so this is secure enough.
			return !string.IsNullOrEmpty(deviceName);
#pragma warning restore CS0162 // Unreachable code detected
		}
		catch
		{
			return false;
		}
	}
}