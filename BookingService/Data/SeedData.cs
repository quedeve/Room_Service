using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BookingService.Data;
using BookingService.Models;

namespace BookingService.Data;
public static class SeedData
{
    public static void Initialize(BookingDbContext context)
    {
        if (!context.Bookings.Any())
        {
            context.Bookings.AddRange(
                new Booking { RoomId = 1, UserId = 1, CheckIn = DateTime.Now, CheckOut = DateTime.Now.AddHours(2) },
                new Booking { RoomId = 2, UserId = 2, CheckIn = DateTime.Now.AddDays(1), CheckOut = DateTime.Now.AddDays(1).AddHours(1) }
                // Add more sample bookings if needed
            );

            context.SaveChanges();
        }
    }
}
