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

    public VideoController(FileAccess fileAccess, Logger logger)
    {
        _fileAccess = fileAccess;
        _logger = logger;
    }
	
    [HttpGet("index")]
    public IActionResult IndexVideos([FromQuery, Required] string deviceName)
    {
        try
        {
            _logger.Log($"Video index requested for {deviceName}.");

            string dir = Path.Combine("Videos", deviceName);
			
            if (!_fileAccess.DirectoryExists(dir))
                return NotFound("Could not find device.");

            string[] files = _fileAccess.GetFiles(dir);

            return Content(JsonSerializer.Serialize(files.Select(Path.GetFileNameWithoutExtension)));
        }
        catch (Exception e)
        {
            _logger.Log("Could not provide video index:");
            _logger.Log(e.ToString());
        }

        return BadRequest("Could not supply video index.");
    }
	
    [HttpGet("download")]
    public IActionResult GetVideo([FromQuery, Required] string deviceName, [FromQuery, Required] string date)
    {
        try
        {
            _logger.Log($"Video requested for {deviceName} at {date}.");

            string dir = Path.Combine("Videos", deviceName, date + ".mp4");
			
            if (!_fileAccess.FileExists(dir))
                return NotFound("Could not find video file.");

            return File(_fileAccess.ReadAllBytes(dir), "video/mp4", date + ".mp4");
        }
        catch (Exception e)
        {
            _logger.Log("Could not provide video:");
            _logger.Log(e.ToString());
        }

        return BadRequest("Could not supply video.");
    }
	
    [HttpPost("upload")]
    public IActionResult UploadVideo([FromForm, Required] IFormFile file)
    {
        try
        {
            _logger.Log($"{Username} requested to upload video \"{file.FileName}\".");
			
            _fileAccess.SaveVideo(Path.Combine("Videos", Username, file.FileName), file);
			
            _logger.Log($"Successfully uploaded video \"{file.FileName}\".");
            return Ok();
        }
        catch (Exception e)
        {
            _logger.Log("ERROR: Could not upload video:");
            _logger.Log(e.ToString());
        }

        return BadRequest();
    }
}