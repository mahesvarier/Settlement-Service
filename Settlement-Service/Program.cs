using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Settlement_Service.Models.Bookings;
using Settlement_Service.Models.Exceptions;
using Settlement_Service.Models.Settings;
using Settlement_Service.Services.Implementations;
using Settlement_Service.Services.Interfaces;
using Settlement_Service.Utilities;
using System.Net;
using System.Text.Json;

#region Building the applicatiom
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("Config/config.json", optional: false, reloadOnChange: true);
builder.Services.Configure<BookingSettings>(builder.Configuration.GetSection("BookingSettings"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IBookingService, BookingService>();
builder.Services.AddSingleton<IBookingRepository, BookingRepository>();

var app = builder.Build();

#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();

#region "API Endpoints"

app.MapPost("/api/booking", async (BookingRequest request, IBookingService bookingService, ILogger<Program> logger) =>
{
    try
    {
        var validationResult = BookingValidator.ValidateBookingRequest(request);

        if (!string.IsNullOrEmpty(validationResult))
        {
            return Results.BadRequest(validationResult);
        }

        TimeSpan.TryParse(request.BookingTime, out TimeSpan bookingTime);

        var bookingResponse = await bookingService.CreateBookingAsync(request.Name, bookingTime);
        logger.LogInformation("Booking created successfully for request: {Request}", JsonSerializer.Serialize(request));

        return Results.Ok(bookingResponse);
    }
    catch (BookingConflictException ex)
    {
        logger.LogWarning(ex, "Booking conflict occurred for request: {Request}", JsonSerializer.Serialize(request));
        return Results.Conflict(ex.ErrorCode);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "InternalServerError occurred for request: {Request}", JsonSerializer.Serialize(request));
        return Results.StatusCode((int)HttpStatusCode.InternalServerError);
    }
});

app.MapGet("/", () => "Running...");
#endregion

app.Run();

