namespace Central_Server.Models;

public class DeviceStatus
{
	public DeviceStatus(Guid trainId, DateTime timestamp, string status)
	{
		TrainId = trainId;
		Timestamp = timestamp;
		Status = status;
	}

	public DateTime Timestamp { get; set; }
	public string Status { get; set; }
	public Guid TrainId { get; set; }
}