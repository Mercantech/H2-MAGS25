namespace DomainModels;
public class Room : Common
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public decimal Price { get; set; }
    public int Capacity { get; set; }

    // Many-to-Many Relations
    public ICollection<BookingRoom> BookingRooms { get; set; } = new List<BookingRoom>();
}