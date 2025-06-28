namespace DomainModels;
public class BookingRoom : Common
{
    public string BookingId { get; set; }
    public Booking Booking { get; set; }
    public string RoomId { get; set; }
    public Room Room { get; set; }
} 