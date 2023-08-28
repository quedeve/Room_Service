# Microservices System Documentation

This documentation provides an overview of the microservices system architecture and the available APIs.

## System Architecture

The microservices system consists of three main services: Booking Service, Rooms Service, and Users Service. These services are accessible through an API Gateway service.

### Services

1. **Booking Service**

   - Base URL: `http://localhost:5002`
   - Responsible for managing bookings.

2. **Rooms Service**

   - Base URL: `http://localhost:5000`
   - Responsible for managing rooms and their availability.

3. **Users Service**

   - Base URL: `http://localhost:5001`
   - Responsible for managing user information.

4. **API Gateway (Ocelot)**
   - Base URL: `http://localhost:5003`
   - Acts as a gateway for routing requests to the appropriate microservices.

## Endpoints

### Booking Service

- `GET /api/bookings`
  - Get all bookings.
- `GET /api/bookings/{id}`
  - Get a booking by ID.
- `POST /api/bookings`
  - Create a new booking.
- `PUT /api/bookings/{id}`
  - Update a booking.
- `DELETE /api/bookings/{id}`
  - Delete a booking.
- `GET /api/bookings/checkroom/{roomId}`
  - Check if a room is available for booking.

### Rooms Service

- `GET /api/rooms`
  - Get all rooms.
- `GET /api/rooms/{id}`
  - Get a room by ID.
- `POST /api/rooms`
  - Create a new room.
- `PUT /api/rooms/{id}`
  - Update a room.
- `DELETE /api/rooms/{id}`
  - Delete a room.

### Users Service

- `GET /api/users`
  - Get all users.
- `GET /api/users/{id}`
  - Get a user by ID.
- `POST /api/users`
  - Create a new user.
- `PUT /api/users/{id}`
  - Update a user.
- `DELETE /api/users/{id}`
  - Delete a user.

## Database Structure

The microservices system uses the following database structure:

- **Booking Service Database**

  - Table: `Bookings`
    - `BookingId` (Primary Key)
    - `RoomId` (Foreign Key referencing Rooms)
    - `UserId`
    - `CheckIn`
    - `CheckOut`

- **Rooms Service Database**

  - Table: `Rooms`
    - `RoomId` (Primary Key)
    - `RoomType`
    - `Capacity`

- **Users Service Database**
  - Table: `Users`
    - `UserId` (Primary Key)
    - `Name`

## Database Creation Script

    ```sql
    -- Create database
    CREATE DATABASE IF NOT EXISTS `bookingsmicroservice` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

    -- Use the database
    USE `bookingsmicroservice`;

    -- Create Rooms table
    CREATE TABLE IF NOT EXISTS `Rooms` (
    `RoomId` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `RoomType` VARCHAR(255) NOT NULL,
    `Capacity` INT NOT NULL
    );

    -- Create Users table
    CREATE TABLE IF NOT EXISTS `Users` (
    `UserId` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(255) NOT NULL
    );

    -- Create Bookings table
    CREATE TABLE IF NOT EXISTS `Bookings` (
    `BookingId` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `RoomId` INT NOT NULL,
    `UserId` INT NOT NULL,
    `CheckIn` DATETIME NOT NULL,
    `CheckOut` DATETIME NOT NULL,
    CONSTRAINT `FK_Rooms_Bookings` FOREIGN KEY (`RoomId`) REFERENCES `Rooms` (`RoomId`),
    CONSTRAINT `FK_Users_Bookings` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`)
    );

## API Gateway (Ocelot) Configuration

The API Gateway service (Ocelot) is responsible for routing requests to the appropriate microservices based on the URL paths. The configuration file is located at `ocelot.json`.

    ```json
    {
        "ReRoutes": [
            {
            "DownstreamPathTemplate": "/api/bookings",
            "DownstreamScheme": "http",
            "DownstreamHostAndPorts": [
                {
                "Host": "localhost",
                "Port": 5002
                }
            ],
            "UpstreamPathTemplate": "/api/bookings",
            "UpstreamHttpMethod": ["GET", "POST", "PUT", "DELETE"]
            },
            {
            "DownstreamPathTemplate": "/api/rooms",
            "DownstreamScheme": "http",
            "DownstreamHostAndPorts": [
                {
                "Host": "localhost",
                "Port": 5000
                }
            ],
            "UpstreamPathTemplate": "/api/rooms",
            "UpstreamHttpMethod": ["GET", "POST", "PUT", "DELETE"]
            },
            {
            "DownstreamPathTemplate": "/api/users",
            "DownstreamScheme": "http",
            "DownstreamHostAndPorts": [
                {
                "Host": "localhost",
                "Port": 5001
                }
            ],
            "UpstreamPathTemplate": "/api/users",
            "UpstreamHttpMethod": ["GET", "POST", "PUT", "DELETE"]
            }
        ],
        "GlobalConfiguration": {
            "BaseUrl": "http://localhost:5003"
        }
    }

## Endpoints

### Booking Service

    ```sh
    # Get all bookings
        curl -X GET http://localhost:5003/api/bookings

        # Get a booking by ID
        curl -X GET http://localhost:5003/api/bookings/{id}

        # Create a new booking
        curl -X POST -H "Content-Type: application/json" -d '{"RoomId": 1, "UserId": 1, "CheckIn": "2023-08-25T08:00:00", "CheckOut": "2023-08-28T10:00:00"}' http://localhost:5003/api/bookings

        # Update a booking
        curl -X PUT -H "Content-Type: application/json" -d '{"RoomId": 1, "UserId": 1, "CheckIn": "2023-08-25T08:00:00", "CheckOut": "2023-08-28T10:00:00"}' http://localhost:5003/api/bookings/{id}

        # Delete a booking
        curl -X DELETE http://localhost:5003/api/bookings/{id}

        # Check if a room is available for booking
        curl -X GET http://localhost:5003/api/bookings/checkroom/{roomId}

### Rooms Service

    ```sh
    # Get all rooms
    curl -X GET http://localhost:5003/api/rooms

    # Get a room by ID
    curl -X GET http://localhost:5003/api/rooms/{id}

    # Create a new room
    curl -X POST -H "Content-Type: application/json" -d '{"RoomType": "Single", "Capacity": 1}' http://localhost:5003/api/rooms

    # Update a room
    curl -X PUT -H "Content-Type: application/json" -d '{"RoomType": "Double", "Capacity": 2}' http://localhost:5003/api/rooms/{id}

    # Delete a room
    curl -X DELETE http://localhost:5003/api/rooms/{id}

    # Check if a room is available for booking
    curl -X GET http://localhost:5003/api/rooms/checkavailability/{id}


 ### Users Service

    ```sh
    # Get all users
    curl -X GET http://localhost:5003/api/users

    # Get a user by ID
    curl -X GET http://localhost:5003/api/users/{id}

    # Create a new user
    curl -X POST -H "Content-Type: application/json" -d '{"Name": "John Doe"}' http://localhost:5003/api/users

    # Update a user
    curl -X PUT -H "Content-Type: application/json" -d '{"Name": "Jane Doe"}' http://localhost:5003/api/users/{id}

    # Delete a user
    curl -X DELETE http://localhost:5003/api/users/{id}

```

```
