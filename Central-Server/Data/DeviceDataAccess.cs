using Central_Server.Models;
using LiteDB;
using Microsoft.VisualBasic.FileIO;

namespace Central_Server.Data;

public class DeviceDataAccess : IDisposable
{
	private readonly string _dataDir;
	private readonly LiteDatabase _database;
	
	public DeviceDataAccess()
	{
#if RELEASE
		_dataDir = "/Data/DeviceData.db";
#else
		_dataDir = Path.Combine(SpecialDirectories.MyDocuments, "AutoTf/CentralServer");
		Directory.CreateDirectory(_dataDir);
		_dataDir += "/DeviceData.db";
#endif
		_database = new LiteDatabase(_dataDir);
		ILiteCollection<DeviceStatus> collection = _database.GetCollection<DeviceStatus>("deviceStatus");
		collection.EnsureIndex(x => x.Username);
	}
	
	// Username is the authentik username
	public void UpdateStatus(string username, string status)
	{
		ILiteCollection<DeviceStatus> collection = _database.GetCollection<DeviceStatus>("deviceStatus");

		DeviceStatus? existingLog = collection.FindOne(x => x.Username == username);
		if (existingLog != null)
		{
			existingLog.Timestamp = DateTime.UtcNow;
			existingLog.Status = status;
			collection.Update(existingLog);
		}
		else
		{
			DeviceStatus newLog = new DeviceStatus
			{
				Username = username,
				Timestamp = DateTime.UtcNow,
				Status = status
			};
			collection.Insert(newLog);
		}
		_database.Checkpoint();
	}
	
	// Username is the authentik username
	public DeviceStatus? GetStatusByName(string username)
	{
		ILiteCollection<DeviceStatus> collection = _database.GetCollection<DeviceStatus>("deviceStatus");

		return collection.FindOne(x => x.Username == username);
	}

	public void CreateTrain(string trainName, string authentikUsername, string trainId)
	{
		ILiteCollection<TrainData> collection = _database.GetCollection<TrainData>("TrainData");
		
		TrainData newLog = new TrainData
		{
			Name = trainName,
			AuthentikUsername = authentikUsername,
			TrainId = trainId,
			CreatedOn = DateTime.Now
		};
		collection.Insert(newLog);
		
		_database.Checkpoint();
	}

	public void DeleteTrain(string trainName, string authentikUsername, string trainId)
	{
		ILiteCollection<TrainData> collection = _database.GetCollection<TrainData>("TrainData");
		collection.DeleteMany(x => x.Name == trainName && x.AuthentikUsername == authentikUsername && x.TrainId == trainId);
		
		_database.Checkpoint();
	}

	public List<TrainData> GetAllTrains()
	{
		ILiteCollection<TrainData> collection = _database.GetCollection<TrainData>("TrainData");
		return collection.FindAll().ToList();
	}

	public void Dispose()
	{
		_database.Dispose();
	}
}