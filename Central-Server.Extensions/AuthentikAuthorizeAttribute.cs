using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Central_Server.Extensions;

public class AuthentikAuthorizeAttribute : Attribute, IAuthorizationFilter
{
	public void OnAuthorization(AuthorizationFilterContext context)
	{
		IHeaderDictionary? headers = context.HttpContext.Request.Headers;

		if (!IsAllowedDevice(headers, out string? deviceName))
		{
			context.Result = new UnauthorizedResult();
		}
		else context.HttpContext.Items["DeviceName"] = deviceName;
	}
	
	private static bool IsAllowedDevice(IHeaderDictionary headers, out string? deviceName)
	{
		deviceName = "placeHolder";
		
		try
		{
			// TODO: Idk if we should do this if DEBUG
#if DEBUG
			return true;
#endif
			deviceName = headers["X-Authentik-Username"].ToString();
			
			// TODO: Validate username and session
			return !string.IsNullOrEmpty(deviceName);
		}
		catch
		{
			// Ignored
		}

		return false;
	}
}