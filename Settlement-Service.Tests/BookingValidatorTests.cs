using Settlement_Service.Models.Bookings;
using Settlement_Service.Utilities;

namespace Settlement_Service.Tests
{
    public class BookingValidatorTests
    {
        [Theory]
        [InlineData(null, "10:00", "INVALID_NAME")]
        [InlineData("Mahes Varier", null, "INVALID_BOOKING_TIME")]
        [InlineData("Mahes Varier", "invalid_time", "INVALID_BOOKING_TIME")]
        [InlineData("Mahes Varier", "08:00", "INVALID_BOOKING_TIME_RANGE")]
        [InlineData("Mahes Varier", "17:00", "INVALID_BOOKING_TIME_RANGE")]
        [InlineData(null, null, "INVALID_REQUEST_DATA")]
        [InlineData("", "", "INVALID_REQUEST_DATA")]
        [InlineData("Mahes Varier", "10:00", "")]
        [InlineData("Mahes Varier", "16:00", "")]
        [InlineData("Mahes Varier", "16:01", "INVALID_BOOKING_TIME_RANGE")]
        [InlineData("Mahes Varier", "1.10:00", "INVALID_BOOKING_TIME")]// Days included
        [InlineData("Mahes Varier", "10:0", "INVALID_BOOKING_TIME")] 
        [InlineData("Mahes Varier", "10:000", "INVALID_BOOKING_TIME")]
        [InlineData("Mahes Varier", "10-00", "INVALID_BOOKING_TIME")]
        [InlineData("Mahes Varier", "1000", "INVALID_BOOKING_TIME")]
        [InlineData("Mahes Varier", "10:00 AM", "INVALID_BOOKING_TIME")]
        [InlineData("Mahes Varier", "25:00", "INVALID_BOOKING_TIME")]
        public void ValidateBookingRequest_ShouldReturnExpectedResult(string name, string bookingTime, string expected)
        {
            // Arrange
            var request = new BookingRequest
            {
                Name = name,
                BookingTime = bookingTime
            };

            // Act
            var result = BookingValidator.ValidateBookingRequest(request);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
