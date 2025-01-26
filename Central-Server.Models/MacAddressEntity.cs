using LiteDB;

namespace Central_Server.Models;

public class MacAddressEntity
{
	public ObjectId Id { get; set; }
	public string Address { get; set; }
}