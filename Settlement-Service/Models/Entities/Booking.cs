namespace Settlement_Service.Models.Entities
{
    public class Booking
    {
        public Guid BookingId { get; set; }
        public string Name { get; set; }
        public TimeSpan BookingTime { get; set; }
    }
}
