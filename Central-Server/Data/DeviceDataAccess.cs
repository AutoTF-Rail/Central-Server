using Central_Server.Models;
using LiteDB;

namespace Central_Server.Data;

public class DeviceDataAccess : IDisposable
{
	// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
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
		collection.EnsureIndex(x => x.TrainId);
		
		ILiteCollection<TrainData> trainCollection = _database.GetCollection<TrainData>("TrainData");
		trainCollection.EnsureIndex(x => x.UniqueId);
	}
	
	public void UpdateStatus(Guid trainId, string status)
	{
		ILiteCollection<DeviceStatus> collection = _database.GetCollection<DeviceStatus>("deviceStatus");

		DeviceStatus? existingLog = collection.FindOne(x => x.TrainId == trainId);
		
		if (existingLog != null)
		{
			existingLog.Timestamp = DateTime.UtcNow;
			existingLog.Status = status;
			collection.Update(existingLog);
		}
		else
		{
			DeviceStatus newLog = new DeviceStatus(trainId, DateTime.UtcNow, status);
			collection.Insert(newLog);
		}
		_database.Checkpoint();
	}
	
	public DeviceStatus? GetStatus(Guid uniqueId)
	{
		ILiteCollection<DeviceStatus> collection = _database.GetCollection<DeviceStatus>("deviceStatus");

		return collection.FindOne(x => x.TrainId == uniqueId);
	}
	
	public bool TrainExists(string authentikUsername)
	{
		ILiteCollection<TrainData> collection = _database.GetCollection<TrainData>("TrainData");

		return collection.FindOne(x => x.AuthentikUsername == authentikUsername) != null;
	}

	public void CreateTrain(string trainName, string authentikUsername, string trainId)
	{
		ILiteCollection<TrainData> collection = _database.GetCollection<TrainData>("TrainData");

		TrainData newLog = new TrainData(trainName, authentikUsername, trainId, DateTime.Now, Guid.NewGuid());
		collection.Insert(newLog);
		
		_database.Checkpoint();
	}

	public bool EditTrain(Guid id, string newTrainName, string newAuthUsername, string newTrainId)
	{
		ILiteCollection<TrainData> collection = _database.GetCollection<TrainData>("TrainData");

		TrainData? train = collection.FindOne(x => x.UniqueId == id);
		
		if (train == null)
			return false;

		train.Name = newTrainName;
		train.AuthentikUsername = newAuthUsername;
		train.TrainId = newTrainId;
		collection.Update(train);
		
		_database.Checkpoint();

		return true;
	}

	/// <summary>
	/// Gets the unique ID of a train by its authentik username.
	/// You usually don't need to check if this return value is Guid.Empty, because most requests won't be reachable if there is no auth username present.
	/// </summary>
	public Guid GetUniqueId(string authUsername)
	{
		ILiteCollection<TrainData> collection = _database.GetCollection<TrainData>("TrainData");
		TrainData? train = collection.FindOne(x => x.AuthentikUsername == authUsername);
		
		return train?.UniqueId ?? Guid.Empty;
	}

	public void DeleteTrain(Guid id)
	{
		ILiteCollection<TrainData> collection = _database.GetCollection<TrainData>("TrainData");
		collection.DeleteMany(x => x.UniqueId == id);
		
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