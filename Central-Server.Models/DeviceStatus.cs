namespace Central_Server.Models;

public class DeviceStatus
{
	public DeviceStatus(string username, DateTime timestamp, string status)
	{
		Username = username;
		Timestamp = timestamp;
		Status = status;
	}

	// TODO: remove?
	public int Id { get; set; }
	public string Username { get; set; }
	public DateTime Timestamp { get; set; }
	public string Status { get; set; }
}