using Settlement_Service.Models.Entities;

namespace Settlement_Service.Services.Interfaces
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetBookingsByTimeRangeAsync(TimeSpan startTime, TimeSpan endTime);
        Task AddBookingAsync(Booking booking);
    }
}
