using System.ComponentModel.DataAnnotations;

namespace Central_Server.Models.RequestBodies;

public class AddTrainBody
{
    public AddTrainBody(string trainName, string authentikUsername, string trainId)
    {
        TrainName = trainName;
        AuthentikUsername = authentikUsername;
        TrainId = trainId;
    }

    [Required]
    public string TrainName { get; set; }
    
    [Required]
    public string AuthentikUsername { get; set; }
    
    [Required]
    public string TrainId { get; set; }
}