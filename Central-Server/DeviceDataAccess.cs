using Central_Server.Models;
using LiteDB;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualBasic.FileIO;

namespace Central_Server;

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
	
	public void UpdateStatus(string username, string status)
	{
		ILiteCollection<DeviceStatus> collection = _database.GetCollection<DeviceStatus>("deviceStatus");

		DeviceStatus existingLog = collection.FindOne(x => x.Username == username);
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
		
	}
	
	public DeviceStatus? GetStatusByName(string username)
	{
		ILiteCollection<DeviceStatus> collection = _database.GetCollection<DeviceStatus>("deviceStatus");

		return collection.FindOne(x => x.Username == username);
	}

	public void Dispose()
	{
		_database.Dispose();
	}
}