namespace DomainModels;
public class Room : Common
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public decimal Price { get; set; }
    public int Capacity { get; set; }

    // Booking mapping with foreign key
    public ICollection<Booking> Bookings { get; set; }
}