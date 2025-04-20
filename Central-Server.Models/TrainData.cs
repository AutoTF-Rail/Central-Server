using LiteDB;

namespace Central_Server.Models;

public class TrainData
{
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
	
	[BsonId]
	public Guid UniqueId { get; set; }
}