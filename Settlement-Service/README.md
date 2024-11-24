# Settlement Service

Settlement Service is a .NET application that provides booking functionalities. It includes validation, logging, and exception handling for booking requests.

## Overview

The Settlement Service allows users to create bookings within a specified time range. It includes validation to ensure the booking time is within the allowed range and follows the correct format.

## Features

- Booking creation with validation
- Logging of booking requests and errors
- Exception handling for booking conflicts and unexpected errors

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)

## Setup

1. Clone the repository:
2. Open the solution in Visual Studio: SettlementService.sln
3. Restore the dependencies: `dotnet restore`

## Usage

	1. Build the project: `dotnet build`
	2. Run the project using IISExpress

	### API Endpoints
	1. View the Swagger documentation for details on endpoint: `https://localhost:44322/swagger/index.html`
	2. Send a POST request to the endpoint: `https://localhost:44322/api/booking`
	3. Include the following JSON payload in the request body:
	```json
	{
		"bookingTime": "10:00",
		"name": "string"
	}
	```
	4. The response will include the booking ID if successful
	5. Check the log file for booking requests and errors
	**Responses**:
        - `200 OK`: Booking created successfully
        - `400 Bad Request`: Invalid request data
        - `409 Conflict`: Booking conflict
        - `500 Internal Server Error`: Unexpected error

## Running Tests

	1. Navigate to the test project directory:
	```bash
		cd SettlementService.Tests
	```
	2. Run the tests:
	```bash
		dotnet test
	```

## Design Considerations

	1. Assumptions:
		- The booking time is in the format "HH:mm"
		- The booking time is within the allowed range (9:00 - 16:00)
		- The booking name is a string that cannot be empty
	2. Configuration:
		- The duration of each spot is configurable in config.json file. Should this time be increased or decreased we can modify the config file and deploy it. Making sure no code is changed (test cases to reflect the same changes)
		- The maximum number of simultaneous bookings is configurable in the config.json file. Should this time be increased or decreased we can modify the config file and deploy it. Making sure no code is changed (test cases to reflect the same changes)
		- The logs can be used to setting alerts or monitoring the application. Should there be more number of Internal Server Errors, we can set up alerts to notify the team.
	3. Front end usage:
		- The front end can be used to create bookings. The front end can be a simple HTML page with a form that sends a POST request to the API endpoint.
		- The BookingId and a Status Code of 200 is enough for the front end to confirm the booking.
		- The application is configured to return specific error codes for different scenarios. For eg: the application can be expanded with the use of different Error codes while returning the same 400 status code. This will help the front end to understand the error and display the same to the user.
	4. Scalability:
		- The application can be scaled horizontally by deploying multiple instances behind a load balancer.
		- The application can be containerized using Docker and deployed to a container orchestration platform like Kubernetes.
	5. **Structure of the project**:
		- Since the project has just one endpoint, the project is structured in a simple way using Minimal API.
		- The project can be expanded with more endpoints and services as needed. 
		- The traditional approach of using controllers was considered but it has been assumed that this project is a microservice doing only one function.
		- The project can be structured using a layered architecture with separate projects for controllers, services, and data access.
	6. **Data storage**:
		- The application does not use a database to store bookings. The bookings are stored in memory as a singleton. Which mean the data will be lost everytime the project is restarted.
		- The data structures considered to store data are:
			- Dictionary: This is used to store the bookings against a booking time.
				- Advantage:
					 The advantage of this is that the retrieval time is O(1).
				- Disadvantage:
					The disadvantage is that the logic of checking for overlapping bookings will be complex.
			- List: This is used to store the bookings.
				- Advantage:
					The advantage of this is that the logic of checking for overlapping bookings will be simple.
				- Disadvantage:
					The disadvantage is that the retrieval time is O(n).
			- Custom Data Structure: A custom data structure can be created to store the bookings.
				- Advantage:
					The advantage of this is that the retrieval time can be O(1) and the logic of checking for overlapping bookings will be simple.
				- Disadvantage:
					The disadvantage is that the implementation will be complex.
			- Considering the above points, the List data structure was chosen to store the bookings since the number of bookings is expected to be small.

