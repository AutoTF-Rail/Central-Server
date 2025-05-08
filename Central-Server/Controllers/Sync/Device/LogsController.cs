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

            string dir = Path.Combine("Logs", id.ToString());

            bool dirExists = _fileAccess.DirectoryExists(dir);
            
            if (!dirExists)
                return new List<string>();

            string[] files = _fileAccess.GetFiles(dir);

            return files.Select(Path.GetFileNameWithoutExtension).ToList()!;
        }
        catch (Exception e)
        {
            _logger.Log($"Could not provide log index for {deviceName}:");
            _logger.Log(e.ToString());
        }

        return BadRequest("Could not supply log index.");
    }
	
    [HttpGet("download")]
    public ActionResult<string[]> GetLogs([FromQuery, Required] string deviceName, [FromQuery, Required] string date)
    {
        try
        {
            Guid id = _deviceDataAccess.GetUniqueId(deviceName);

            if (id == Guid.Empty)
                return NotFound("Could not find device.");

            string dir = Path.Combine("Logs", id.ToString(), date + ".txt");
			
            if (!_fileAccess.FileExists(dir))
                return NotFound("Could not find log file.");

            return _fileAccess.ReadAllLines(dir);
        }
        catch (Exception e)
        {
            _logger.Log($"Could not provide logs for {deviceName}:");
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
            
            _fileAccess.AppendAllLines(Path.Combine("Logs", id.ToString(), DateTime.Now.ToString("yyyy-MM-dd") + ".txt"),
                logs);
            
            return Ok();
        }
        catch (Exception e)
        {
            _logger.Log($"Could not upload logs for {Username}:");
            _logger.Log(e.ToString());
        }

        return BadRequest();
    }
}