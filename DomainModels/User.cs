using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace DomainModels;
public class User : Common
{
    [Required(ErrorMessage = "Navn er påkrævet.")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Email er påkrævet.")]
    [EmailAddress(ErrorMessage = "Ugyldig email-adresse.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Adgangskode er påkrævet.")]
    [MinLength(6, ErrorMessage = "Adgangskode skal være mindst 6 tegn.")]
    public string PasswordHash { get; set; }
    public ICollection<BookingUser> BookingUsers { get; set; } = new List<BookingUser>();
}
