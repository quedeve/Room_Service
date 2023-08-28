using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;

public static class SeedData
{
    public static void Initialize(UserDbContext context)
    {
        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User { Name = "John Doe" },
                new User { Name = "Jane Smith" }
                // Add more sample users if needed
            );

            context.SaveChanges();
        }
    }
}