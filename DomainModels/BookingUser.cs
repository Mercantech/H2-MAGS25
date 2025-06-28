namespace DomainModels;
public class BookingUser : Common
{
    public string BookingId { get; set; }
    public Booking Booking { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
} 