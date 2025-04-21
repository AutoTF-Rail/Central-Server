using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using AutoTf.Logging;
using Central_Server.Data;
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
    private readonly DeviceDataAccess _deviceDataAccess;

    public LogsController(FileAccess fileAccess, Logger logger, DeviceDataAccess deviceDataAccess)
    {
        _fileAccess = fileAccess;
        _logger = logger;
        _deviceDataAccess = deviceDataAccess;
    }

    [HttpGet("index")]
    public ActionResult<List<string>> IndexLogs([FromQuery, Required] string deviceName)
    {
        try
        {
            if (!_deviceDataAccess.TrainExists(deviceName))
                return NotFound("Could not find device.");
            
            Guid id = _deviceDataAccess.GetUniqueId(deviceName);
            _logger.Log($"[{id.ToString()}] Logs index requested.");

            string dir = Path.Combine("Logs", deviceName);

            bool dirExists = _fileAccess.DirectoryExists(dir);
            
            if (!dirExists)
                return new List<string>();

            string[] files = _fileAccess.GetFiles(dir);

            return files.Select(Path.GetFileNameWithoutExtension).ToList()!;
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
            Guid id = _deviceDataAccess.GetUniqueId(deviceName);

            if (id == Guid.Empty)
                return NotFound("Could not find device.");
            
            _logger.Log($"Logs requested for {id.ToString()} at {date}.");

            string dir = Path.Combine("Logs", id.ToString(), date + ".txt");
			
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
            Guid id = _deviceDataAccess.GetUniqueId(Username);
            
            _logger.Log($"{id.ToString()} requested to upload logs.");
            
            _fileAccess.AppendAllLines(Path.Combine("Logs", id.ToString(), DateTime.Now.ToString("yyyy-MM-dd") + ".txt"),
                logs);
			
            _logger.Log($"Successfully uploaded logs for {id.ToString()}.");
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