using Microsoft.Extensions.Options;
using Settlement_Service.Models.Bookings;
using Settlement_Service.Models.Common;
using Settlement_Service.Models.Entities;
using Settlement_Service.Models.Exceptions;
using Settlement_Service.Models.Settings;
using Settlement_Service.Services.Interfaces;

namespace Settlement_Service.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly BookingSettings _bookingSettings;

        public BookingService(IBookingRepository bookingRepository, IOptions<BookingSettings> bookingSettings)
        {
            _bookingRepository = bookingRepository;
            _bookingSettings = bookingSettings.Value;
        }

        #region Creating the booking
        public async Task<BookingResponse> CreateBookingAsync(string name, TimeSpan bookingTime)
        {
            var startTime = bookingTime;
            var endTime = bookingTime.Add(TimeSpan.FromHours(_bookingSettings.SlotDurationInHours));

            var existingBookings = await _bookingRepository.GetBookingsByTimeRangeAsync(startTime, endTime);

            if (existingBookings.Count >= _bookingSettings.MaxSimultaneousBookings)
            {
                throw new BookingConflictException(ErrorCodes.ALL_SLOTS_FOR_TIME_BOOKED);
            }

            var booking = new Booking
            {
                BookingId = Guid.NewGuid(),
                Name = name,
                BookingTime = bookingTime
            };

            await _bookingRepository.AddBookingAsync(booking);

            var bookingResponse = new BookingResponse
            {
                BookingId = booking.BookingId,
            };

            return bookingResponse;
        }
        #endregion
    }
}
