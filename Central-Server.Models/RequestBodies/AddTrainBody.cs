using System.ComponentModel.DataAnnotations;

namespace Central_Server.Models.RequestBodies;

public class AddTrainBody
{
    [Required]
    public string TrainName { get; set; }
    
    [Required]
    public string AuthentikUsername { get; set; }
    
    [Required]
    public string TrainId { get; set; }
}