using LiteDB;

namespace Central_Server.Models;

public class MacAddressEntity
{
	public MacAddressEntity(string address)
	{
		Address = address;
	}

	public ObjectId Id { get; set; } = null!;
	public string Address { get; set; }
}