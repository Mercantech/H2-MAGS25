using System.Collections.Generic;
namespace DomainModels;
public class User : Common
{
    public string Name { get; set; }
    public ICollection<BookingUser> BookingUsers { get; set; } = new List<BookingUser>();
}
