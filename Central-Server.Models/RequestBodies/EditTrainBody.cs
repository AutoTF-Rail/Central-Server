using System.ComponentModel.DataAnnotations;

namespace Central_Server.Models.RequestBodies;

public class EditTrainBody
{
    public EditTrainBody(Guid id, string trainName, string authentikUsername, string trainId)
    {
        Id = id;
        TrainName = trainName;
        AuthentikUsername = authentikUsername;
        TrainId = trainId;
    }

    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public string TrainName { get; set; }
    
    [Required]
    public string AuthentikUsername { get; set; }
    
    [Required]
    public string TrainId { get; set; }
}