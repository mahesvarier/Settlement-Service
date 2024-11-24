using Microsoft.AspNetCore.Http;
using Moq;
using Settlement_Service.Utilities;
using System.Net;
using System.Text.Json;

namespace Settlement_Service.Tests
{
    public class ExceptionHandlingMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_ShouldReturn400_WhenBadHttpRequestExceptionIsThrown()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            var mockRequestDelegate = new Mock<RequestDelegate>();
            mockRequestDelegate
                .Setup(rd => rd(It.IsAny<HttpContext>()))
                .Throws(new BadHttpRequestException("Invalid request"));

            var middleware = new ExceptionHandlingMiddleware(mockRequestDelegate.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = new StreamReader(responseBodyStream).ReadToEnd();
            var expectedResponse = JsonSerializer.Serialize(new
            {
                ErrorCode = "INVALID_REQUEST_BODY",
                Message = "The request body is not a valid JSON."
            });
            Assert.Equal(expectedResponse, responseBody);
        }

        [Fact]
        public async Task InvokeAsync_ShouldCallNextMiddleware_WhenNoExceptionIsThrown()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var mockRequestDelegate = new Mock<RequestDelegate>();
            mockRequestDelegate
                .Setup(rd => rd(It.IsAny<HttpContext>()))
                .Returns(Task.CompletedTask);

            var middleware = new ExceptionHandlingMiddleware(mockRequestDelegate.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
            mockRequestDelegate.Verify(rd => rd(It.IsAny<HttpContext>()), Times.Once);
        }
    }
}
