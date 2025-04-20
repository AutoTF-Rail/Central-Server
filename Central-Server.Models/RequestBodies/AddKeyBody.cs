using System.ComponentModel.DataAnnotations;

namespace Central_Server.Models.RequestBodies;

public class AddKeyBody
{
    [Required]
    public string SerialNumber { get; set; }

    [Required]
    public string Secret { get; set; }
}