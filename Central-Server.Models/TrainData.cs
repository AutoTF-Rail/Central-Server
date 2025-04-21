using LiteDB;

namespace Central_Server.Models;

public class TrainData
{
	
	// LiteDB requires this
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public TrainData() {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	
	public TrainData(string name, string authentikUsername, string trainId, DateTime createdOn, Guid uniqueId)
	{
		Name = name;
		AuthentikUsername = authentikUsername;
		TrainId = trainId;
		CreatedOn = createdOn;
		UniqueId = uniqueId;
	}

	public string Name { get; set; }
	public string AuthentikUsername { get; set; }
	public string TrainId { get; set; }
	public DateTime CreatedOn { get; set; }
	
	public Guid UniqueId { get; set; }

	public ObjectId Id { get; set; } = null!;
}