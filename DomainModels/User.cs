using System.Collections.Generic;
namespace DomainModels;
public class User : Common
{
    public string Name { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
