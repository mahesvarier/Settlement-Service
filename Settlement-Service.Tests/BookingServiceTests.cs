using Microsoft.Extensions.Options;
using Moq;
using Settlement_Service.Models.Common;
using Settlement_Service.Models.Entities;
using Settlement_Service.Models.Exceptions;
using Settlement_Service.Models.Settings;
using Settlement_Service.Services.Implementations;
using Settlement_Service.Services.Interfaces;

namespace Settlement_Service.Tests
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockBookingRepository;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _mockBookingRepository = new Mock<IBookingRepository>();

            var bookingSettings = new BookingSettings
            {
                MaxSimultaneousBookings = 4,
                SlotDurationInHours = 1
            };

            var mockOptions = new Mock<IOptions<BookingSettings>>();
            mockOptions.Setup(o => o.Value).Returns(bookingSettings);

            _bookingService = new BookingService(_mockBookingRepository.Object, mockOptions.Object);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldCreateBooking_WhenSlotsAreAvailable()
        {
            // Arrange
            var bookingTime = new TimeSpan(9, 0, 0);
            var name = "Mahes Varier";
            _mockBookingRepository
                .Setup(repo => repo.GetBookingsByTimeRangeAsync(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(new List<Booking>());

            // Act
            var bookingResponse = await _bookingService.CreateBookingAsync(name, bookingTime);

            // Assert
            Assert.NotNull(bookingResponse);
            Assert.NotEqual(Guid.Empty, bookingResponse.BookingId);
            _mockBookingRepository.Verify(repo => repo.AddBookingAsync(It.Is<Booking>(b => b.Name == name && b.BookingTime == bookingTime)), Times.Once);
        }

        [Theory]
        [InlineData("09:00:00")]
        [InlineData("09:05:00")]
        [InlineData("09:30:00")]
        [InlineData("09:59:00")]
        public async Task CreateBookingAsync_ShouldThrowException_WhenSlotsAreFull(string bookingTimeString)
        {
            // Arrange
            var bookingTime = TimeSpan.Parse(bookingTimeString);
            var name = "Mahes Varier";
            var existingBookings = new List<Booking>
            {
                new Booking { BookingTime = new TimeSpan(9, 0, 0) },
                new Booking { BookingTime = new TimeSpan(9, 0, 0) },
                new Booking { BookingTime = new TimeSpan(9, 0, 0) },
                new Booking { BookingTime = new TimeSpan(9, 0, 0) }
            };
            _mockBookingRepository
                .Setup(repo => repo.GetBookingsByTimeRangeAsync(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(existingBookings);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BookingConflictException>(() => _bookingService.CreateBookingAsync(name, bookingTime));
            Assert.Equal(ErrorCodes.ALL_SLOTS_FOR_TIME_BOOKED, exception.ErrorCode);
            _mockBookingRepository.Verify(repo => repo.AddBookingAsync(It.IsAny<Booking>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldCreateBooking_WhenThereAreFewerThan4Bookings()
        {
            // Arrange
            var bookingTime = new TimeSpan(9, 0, 0);
            var name = "Mahes Varier";
            var existingBookings = new List<Booking>
            {
                new Booking { BookingTime = new TimeSpan(9, 0, 0) },
                new Booking { BookingTime = new TimeSpan(9, 0, 0) },
                new Booking { BookingTime = new TimeSpan(9, 0, 0) }
            };
            _mockBookingRepository
                .Setup(repo => repo.GetBookingsByTimeRangeAsync(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(existingBookings);

            // Act
            var bookingResponse = await _bookingService.CreateBookingAsync(name, bookingTime);

            // Assert
            Assert.NotNull(bookingResponse);
            Assert.NotEqual(Guid.Empty, bookingResponse.BookingId);
            _mockBookingRepository.Verify(repo => repo.AddBookingAsync(It.IsAny<Booking>()), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowException_WhenThereAreOverlappingBookings()
        {
            // Arrange
            var bookingTime = new TimeSpan(9, 30, 0);
            var name = "Mahes Varier";
            var existingBookings = new List<Booking>
            {
                new Booking { BookingTime = new TimeSpan(9, 0, 0) },
                new Booking { BookingTime = new TimeSpan(9, 15, 0) },
                new Booking { BookingTime = new TimeSpan(9, 30, 0) },
                new Booking { BookingTime = new TimeSpan(9, 45, 0) }
            };
            _mockBookingRepository
                .Setup(repo => repo.GetBookingsByTimeRangeAsync(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(existingBookings);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BookingConflictException>(() => _bookingService.CreateBookingAsync(name, bookingTime));
            Assert.Equal(ErrorCodes.ALL_SLOTS_FOR_TIME_BOOKED, exception.ErrorCode);
            _mockBookingRepository.Verify(repo => repo.AddBookingAsync(It.IsAny<Booking>()), Times.Never);
        }
    }
}
