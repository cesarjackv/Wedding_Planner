#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Planning
{
    [Key]
    public int PlanningId {get; set;}
    [Required]
    public int UserId {get; set;}
    [Required]
    public int WeddingId {get; set;}
    public User? User {get; set;}
    public Wedding? Wedding {get; set;}
}