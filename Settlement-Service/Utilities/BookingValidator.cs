using Settlement_Service.Models.Bookings;
using System.Globalization;

namespace Settlement_Service.Utilities
{
    public static class BookingValidator
    {
        public static string ValidateBookingRequest(BookingRequest request)
        {
            if (request == null || ((string.IsNullOrEmpty(request.Name)) && string.IsNullOrEmpty(request.BookingTime)))
            {
                return "INVALID_REQUEST_DATA";
            }

            if (string.IsNullOrEmpty(request.Name))
            {
                return "INVALID_NAME";
            }

            if (string.IsNullOrEmpty(request.BookingTime))
            {
                return "INVALID_BOOKING_TIME";
            }

            var bookingTimeStr = request.BookingTime.Trim();

            //ensuring that the format is strictly "HH:mm"
            if (!TimeSpan.TryParseExact(bookingTimeStr, "hh\\:mm", CultureInfo.InvariantCulture, out var bookingTime))
            {
                return "INVALID_BOOKING_TIME";
            }

            //ensure the TimeSpan does not include days
            if (bookingTime.Days > 0)
            {
                return "INVALID_BOOKING_TIME_RANGE";
            }

            if (bookingTime < TimeSpan.FromHours(9) || bookingTime > TimeSpan.FromHours(16))
            {
                return "INVALID_BOOKING_TIME_RANGE";
            }

            return string.Empty;
        }
    }
}
