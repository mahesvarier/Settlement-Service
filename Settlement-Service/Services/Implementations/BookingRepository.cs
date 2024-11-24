using Settlement_Service.Models.Entities;
using Settlement_Service.Services.Interfaces;

namespace Settlement_Service.Services.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly List<Booking> _bookings = [];

        public Task<List<Booking>> GetBookingsByTimeRangeAsync(TimeSpan startTime, TimeSpan endTime)
        {
            var bookingsInRange = _bookings
                .Where(booking =>
                    booking.BookingTime < endTime &&
                    booking.BookingTime.Add(TimeSpan.FromHours(1)) > startTime
                )
                .ToList();

            return Task.FromResult(bookingsInRange);
        }

        public Task AddBookingAsync(Booking booking)
        {
            _bookings.Add(booking);
            return Task.CompletedTask;
        }
    }
}