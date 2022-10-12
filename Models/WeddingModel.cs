#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Wedding
{
    [Key]
    public int WeddingId {get; set;}


    // public class DoBAttribute : ValidationAttribute
    // {
    //     public override bool IsValid(object? value1)
    //     {
    //         if(value1 == null)
    //         {
    //             return true;
    //         }
    //         var val = (DateTime)value1;
    //         return (val.AddYears(18) < DateTime.Now);
    //     }
    // }

    [Required]
    public int UserId {get; set;}

    public User? User {get; set;}




    [Required(ErrorMessage = "is required.")]
    [MinLength(2, ErrorMessage = "must be at least 2 characters.")]
    [Display(Name = "Wedder One")]
    public string Wedder1 {get; set;}

    [Required(ErrorMessage = "is required.")]
    [MinLength(2, ErrorMessage = "must be at least 2 characters.")]
    [Display(Name = "Wedder Two")]
    public string Wedder2 {get; set;}




    public class FutureDate : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext value1)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            DateTime date = (DateTime)value;

            if (date < DateTime.Now)
            {
                return new ValidationResult("must be in the future");
            }
            return ValidationResult.Success;
        }
    }

    [Required(ErrorMessage = "is required.")]
    [DataType(DataType.Date)]
    [FutureDate]
    public DateTime Date {get; set;}


    [Required(ErrorMessage = "is required.")]
    public string Address {get; set;}



    public List<Planning> Guests {get; set;} = new List<Planning> ();





    public DateTime CreatedAt {get; set;} = DateTime.Now;

    public DateTime UpdatedAt {get; set;} = DateTime.Now;
}


