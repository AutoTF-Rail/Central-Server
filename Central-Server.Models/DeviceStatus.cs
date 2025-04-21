using LiteDB;

namespace Central_Server.Models;

public class DeviceStatus
{
	// LiteDB requires this
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public DeviceStatus() {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public DeviceStatus(Guid trainId, DateTime timestamp, string status)
	{
		TrainId = trainId;
		Timestamp = timestamp;
		Status = status;
	}

	public DateTime Timestamp { get; set; }
	public string Status { get; set; }
	public Guid TrainId { get; set; }

	public ObjectId Id { get; set; } = null!;
}