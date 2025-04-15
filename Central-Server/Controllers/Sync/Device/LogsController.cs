using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using AutoTf.Logging;
using Central_Server.Extensions;
using Microsoft.AspNetCore.Mvc;
using FileAccess = Central_Server.Data.FileAccess;

namespace Central_Server.Controllers.Sync.Device;

[ApiController]
[Route("/sync/device/logs")]
public class LogsController : AuthentikController
{
    private readonly FileAccess _fileAccess;
    private readonly Logger _logger;

    public LogsController(FileAccess fileAccess, Logger logger)
    {
        _fileAccess = fileAccess;
        _logger = logger;
    }

    [HttpGet("index")]
    public IActionResult IndexLogs([FromQuery, Required] string deviceName)
    {
        try
        {
            _logger.Log($"Logs index requested for {deviceName}.");

            string dir = Path.Combine("Logs", deviceName);
			
            if (!_fileAccess.DirectoryExists(dir))
                return NotFound("Could not find device.");

            string[] files = _fileAccess.GetFiles(dir);

            return Content(JsonSerializer.Serialize(files.Select(Path.GetFileNameWithoutExtension)));
        }
        catch (Exception e)
        {
            _logger.Log("Could not provide log index:");
            _logger.Log(e.ToString());
        }

        return BadRequest("Could not supply log index.");
    }
	
    [HttpGet("download")]
    public IActionResult GetLogs([FromQuery, Required] string deviceName, [FromQuery, Required] string date)
    {
        try
        {
            _logger.Log($"Logs requested for {deviceName} at {date}.");

            string dir = Path.Combine("Logs", deviceName, date + ".txt");
			
            if (!_fileAccess.FileExists(dir))
                return NotFound("Could not find log file.");

            return Content(JsonSerializer.Serialize(_fileAccess.ReadAllLines(dir)));
        }
        catch (Exception e)
        {
            _logger.Log("Could not provide logs:");
            _logger.Log(e.ToString());
        }

        return BadRequest("Could not supply logs.");
    }
	
    [HttpPost("upload")]
    public IActionResult UploadLogs([FromBody, Required] string[] logs)
    {
        try
        {
            _logger.Log($"{Username} requested to upload logs.");
            
            _fileAccess.AppendAllLines(Path.Combine("Logs", Username, DateTime.Now.ToString("yyyy-MM-dd") + ".txt"),
                logs);
			
            _logger.Log($"Successfully uploaded logs for {Username}.");
            return Ok();
        }
        catch (Exception e)
        {
            _logger.Log("ERROR: Could not upload logs:");
            _logger.Log(e.ToString());
        }

        return BadRequest();
    }
}