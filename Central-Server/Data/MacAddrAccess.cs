using Central_Server.Models;
using LiteDB;
using Microsoft.VisualBasic.FileIO;

namespace Central_Server.Data;

public class MacAddrAccess
{
	// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
	private readonly string _dataDir;
	private readonly LiteDatabase _database;
	
	private const string LastChangedId = "LastChanged";
	
	public MacAddrAccess()
	{
#if RELEASE
		_dataDir = "/Data/MacAddrData.db";
#else
		_dataDir = Path.Combine(SpecialDirectories.MyDocuments, "AutoTf/CentralServer");
		Directory.CreateDirectory(_dataDir);
		_dataDir += "/MacAddrData.db";
#endif
		_database = new LiteDatabase(_dataDir);
		ILiteCollection<MacAddressEntity> collection = _database.GetCollection<MacAddressEntity>("macAddrData");
		collection.EnsureIndex(x => x.Id);
		
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
		Dictionary<string, object> document = (Dictionary<string, object>)collection.FindById(LastChangedId); 
		return (DateTime)document["Date"];  
	}
	
	public List<string> GetAll()
	{
		ILiteCollection<MacAddressEntity> collection = _database.GetCollection<MacAddressEntity>("macAddrData");
		return collection.FindAll().Select(x => x.Address).ToList();
	}

	public void CreateAddress(string address)
	{
		ILiteCollection<MacAddressEntity> collection = _database.GetCollection<MacAddressEntity>("macAddrData");

		collection.Insert(new MacAddressEntity(address));

		UpdateLastChanged();
		_database.Checkpoint();
	}

	public void Dispose()
	{
		_database.Dispose();
	}
}