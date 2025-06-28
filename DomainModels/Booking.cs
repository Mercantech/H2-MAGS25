namespace DomainModels;
public class Booking : Common
{
    // Booking Details
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }

    // Many-to-Many Relations
    public ICollection<BookingUser> BookingUsers { get; set; } = new List<BookingUser>();
    public ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();

    public decimal CalculateTotalPrice()
    {
        if (!BookingRooms.Any())
            return 0;

        int numberOfDays = (CheckOutDate - CheckInDate).Days;
        return BookingRooms.Sum(br => br.Room.Price * numberOfDays);
    }
}