using LiteDB;

namespace Central_Server.Models;

public class KeyData
{
	public required string SerialNumber { get; set; }
	public DateTime LastUsed { get; set; }
	public DateTime? DeletedOn { get; set; }
	public bool Verified { get; set; }
	public required string Secret { get; set; }
	public required DateTime CreatedOn { get; set; }

	public ObjectId Id { get; set; } = null!;
}