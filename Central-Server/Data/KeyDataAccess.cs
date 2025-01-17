using Central_Server.Models;
using LiteDB;
using Microsoft.VisualBasic.FileIO;
using OtpNet;

namespace Central_Server.Data;

public class KeyDataAccess : IDisposable
{
	private readonly string _dataDir;
	private readonly LiteDatabase _database;
	
	private const string LastChangedId = "LastChanged";
	
	public KeyDataAccess()
	{
#if RELEASE
		_dataDir = "/Data/KeyData.db";
#else
		_dataDir = Path.Combine(SpecialDirectories.MyDocuments, "AutoTf/CentralServer");
		Directory.CreateDirectory(_dataDir);
		_dataDir += "/KeyData.db";
#endif
		_database = new LiteDatabase(_dataDir);
		ILiteCollection<KeyData> collection = _database.GetCollection<KeyData>("keyData");
		collection.EnsureIndex(x => x.SerialNumber);
		ILiteCollection<object> settings = _database.GetCollection<object>("DataSettings");
		InitializeDatabase();
	}

	private void InitializeDatabase()
	{
		BsonDocument entity = new BsonDocument { ["Date"] = DateTime.Now };
		ILiteCollection<object> collection = _database.GetCollection<object>("DataSettings");
		if (collection.FindById("LastChanged") == null)
			collection.Insert(LastChangedId, entity);
	}

	// The changes that require this method should be so substantial, that the method itself should save the database at the end, so this method doesn't require a checkpoint call.
	private void UpdateLastChanged()
	{
		ILiteCollection<object> collection = _database.GetCollection<object>("DataSettings");
		BsonDocument entity = new BsonDocument { ["Date"] = DateTime.Now };
		collection.Update(LastChangedId,  entity);
	}

	public DateTime GetLastChanged()
	{
		ILiteCollection<object> collection = _database.GetCollection<object>("DataSettings");
		return (DateTime)collection.FindById("LastChanged");
	}
	
	public List<KeyData> GetNew(DateTime lastSync)
	{
		ILiteCollection<KeyData> collection = _database.GetCollection<KeyData>("KeyData");
		return collection.Find(x => x.Verified && x.CreatedOn > lastSync && (x.DeletedOn == null || x.DeletedOn > lastSync)).ToList();
	}
	
	public List<KeyData> GetAll()
	{
		ILiteCollection<KeyData> collection = _database.GetCollection<KeyData>("KeyData");
		return collection.FindAll().ToList();
	}

	public bool CheckForKey(string serialNumber)
	{
		ILiteCollection<KeyData> collection = _database.GetCollection<KeyData>("KeyData");

		KeyData? key = collection.FindOne(x => x.SerialNumber == serialNumber);
		return key != null;
	}

	// the last used value doesn't matter to trains, so we don't have to change the "LastChanged" value for the entire table
	public void UpdateLastUsed(string serialNumber, DateTime date)
	{
		ILiteCollection<KeyData> collection = _database.GetCollection<KeyData>("KeyData");

		KeyData? key = collection.FindOne(x => x.SerialNumber == serialNumber);
		key.LastUsed = date;
		collection.Update(key);
		_database.Checkpoint();
	}

	public void UpdateDeletedOn(string serialNumber, DateTime? date)
	{
		ILiteCollection<KeyData> collection = _database.GetCollection<KeyData>("KeyData");

		KeyData? key = collection.FindOne(x => x.SerialNumber == serialNumber);
		key.DeletedOn = date;
		collection.Update(key);
		UpdateLastChanged();
		_database.Checkpoint();
	}

	public void UpdateVerified(string serialNumber, bool verified)
	{
		ILiteCollection<KeyData> collection = _database.GetCollection<KeyData>("KeyData");

		KeyData? key = collection.FindOne(x => x.SerialNumber == serialNumber);
		key.Verified = verified;
		collection.Update(key);
		UpdateLastChanged();
		_database.Checkpoint();
	}
	
	public void CreateKey(string serialNumber, string secret)
	{
		ILiteCollection<KeyData> collection = _database.GetCollection<KeyData>("KeyData");
		
		KeyData newLog = new KeyData
		{
			SerialNumber = serialNumber,
			Secret = secret,
			LastUsed = DateTime.Now,
			Verified = true,
			CreatedOn = DateTime.Now
		};
		collection.Insert(newLog);
		UpdateLastChanged();
		_database.Checkpoint();
	}
	
	public bool GetVerified(string serialNumber)
	{
		ILiteCollection<KeyData> collection = _database.GetCollection<KeyData>("KeyData");

		return collection.FindOne(x => x.SerialNumber == serialNumber).Verified;
	}
	
	public DateTime GetLastUsed(string serialNumber)
	{
		ILiteCollection<KeyData> collection = _database.GetCollection<KeyData>("KeyData");

		return collection.FindOne(x => x.SerialNumber == serialNumber).LastUsed;
	}
	
	public DateTime? GetDeletedOn(string serialNumber)
	{
		ILiteCollection<KeyData> collection = _database.GetCollection<KeyData>("KeyData");

		return collection.FindOne(x => x.SerialNumber == serialNumber).DeletedOn;
	}
	
	public string GetSecret(string serialNumber)
	{
		ILiteCollection<KeyData> collection = _database.GetCollection<KeyData>("KeyData");

		return collection.FindOne(x => x.SerialNumber == serialNumber).Secret;
	}

	public void Dispose()
	{
		_database.Dispose();
	}

	public bool Validate(string serialNumber, string code, DateTime timestamp)
	{
		string secret = GetSecret(serialNumber);
		byte[] secretBytes = Base32Encoding.ToBytes(secret);

		Totp totp = new Totp(secretBytes);
		
		return totp.ComputeTotp(timestamp) == code;
	}
}