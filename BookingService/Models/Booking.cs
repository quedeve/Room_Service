namespace BookingService.Models;
public class Booking
{
    public int BookingId { get; set; }
    public int RoomId { get; set; }
    public int UserId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }

}
