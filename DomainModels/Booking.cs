namespace DomainModels;
public class Booking : Common
{
    // User and Room mapping with foreign key
    public string UserId { get; set; }
    public User User { get; set; }
    public string RoomId { get; set; }
    public Room Room { get; set; }

    // Booking Details
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }

    // Price Details
    public decimal TotalPrice { get; set; }

    public decimal CalculateTotalPrice()
    {
        if (Room == null)
            return 0;

        int numberOfDays = (CheckOutDate - CheckInDate).Days;
        return Room.Price * numberOfDays;
    }
}