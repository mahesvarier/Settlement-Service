using Settlement_Service.Models.Bookings;

namespace Settlement_Service.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateBookingAsync(string name, TimeSpan bookingTime);
    }
}
