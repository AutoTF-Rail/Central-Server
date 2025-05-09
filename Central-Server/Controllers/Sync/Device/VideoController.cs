using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using AutoTf.Logging;
using Central_Server.Data;
using Central_Server.Extensions;
using Microsoft.AspNetCore.Mvc;
using FileAccess = Central_Server.Data.FileAccess;

namespace Central_Server.Controllers.Sync.Device;

[ApiController]
[Route("/sync/device/video")]
public class VideoController : AuthentikController
{
    private readonly FileAccess _fileAccess;
    private readonly Logger _logger;
    private readonly DeviceDataAccess _deviceDataAccess;
    
    public VideoController(FileAccess fileAccess, Logger logger, DeviceDataAccess deviceDataAccess)
    {
        _fileAccess = fileAccess;
        _logger = logger;
        _deviceDataAccess = deviceDataAccess;
    }
	
    [HttpGet("index")]
    public ActionResult<List<string>> IndexVideos([FromQuery, Required] string deviceName)
    {
        try
        {
            if (!_deviceDataAccess.TrainExists(deviceName))
                return NotFound("Could not find device.");
            
            Guid id = _deviceDataAccess.GetUniqueId(deviceName);

            string dir = Path.Combine("Videos", id.ToString());
            
            bool dirExists = _fileAccess.DirectoryExists(dir);

            if (!dirExists)
                return new List<string>();

            string[] files = _fileAccess.GetFiles(dir);

            return files.Select(Path.GetFileNameWithoutExtension).ToList()!;
        }
        catch (Exception e)
        {
            _logger.Log($"Could not provide video index for {deviceName}:");
            _logger.Log(e.ToString());
        }

        return BadRequest("Could not supply video index.");
    }
	
    [HttpGet("download")]
    public IActionResult GetVideo([FromQuery, Required] string deviceName, [FromQuery, Required] string date)
    {
        try
        {
            Guid id = _deviceDataAccess.GetUniqueId(deviceName);

            if (id == Guid.Empty)
                return NotFound("Could not find device.");

            string dir = Path.Combine("Videos", id.ToString(), date + ".mp4");
			
            if (!_fileAccess.FileExists(dir))
                return NotFound("Could not find video file.");

            return File(_fileAccess.ReadAllBytes(dir), "video/mp4", $"{deviceName}-{date}.mp4");
        }
        catch (Exception e)
        {
            _logger.Log($"Could not provide video for {deviceName}:");
            _logger.Log(e.ToString());
        }

        return BadRequest("Could not supply video.");
    }
    
    [HttpPost("upload")]
    public IActionResult UploadVideo([FromForm, Required] IFormFile file)
    {
        try
        {
            Guid id = _deviceDataAccess.GetUniqueId(Username);
            
            _fileAccess.SaveVideo(Path.Combine("Videos", id.ToString(), file.FileName), file);
            
            return Ok();
        }
        catch (Exception e)
        {
            _logger.Log($"Could not upload video for {Username}:");
            _logger.Log(e.ToString());
        }

        return BadRequest();
    }
}