using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BookingService.Data;
using BookingService.Models;

namespace BookingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly BookingDbContext _context;

    public BookingsController(BookingDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
    {
        return await _context.Bookings.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Booking>> GetBooking(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
        {
            return NotFound();
        }

        return booking;
    }

    [HttpPost]
    public async Task<ActionResult<Booking>> CreateBooking(Booking booking)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingBookingForRoom = await _context.Bookings
                .Where(b => b.RoomId == booking.RoomId)
                .FirstOrDefaultAsync(b => b.CheckIn <= booking.CheckOut && b.CheckOut >= booking.CheckIn);

            if (existingBookingForRoom != null)
            {
                ModelState.AddModelError("", "The room is already booked for the given dates.");
                return BadRequest(ModelState);
            }

            // Create a new booking and save it to the database
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookings), new { id = booking.BookingId }, booking);
        }
        catch (System.Exception)
        {

            return BadRequest();
        }

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBooking(int id, Booking booking)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingBooking = await _context.Bookings.FindAsync(id);

        if (existingBooking == null)
        {
            return NotFound();
        }

        var existingBookingForRoom = await _context.Bookings
            .Where(b => b.RoomId == booking.RoomId && b.BookingId != id)
            .FirstOrDefaultAsync(b => b.CheckIn <= booking.CheckOut && b.CheckOut >= booking.CheckIn);

        if (existingBookingForRoom != null)
        {
            ModelState.AddModelError("", "The room is already booked for the given dates.");
            return BadRequest(ModelState);
        }

        // Update the booking and save changes
        existingBooking.RoomId = booking.RoomId;
        existingBooking.UserId = booking.UserId;
        existingBooking.CheckIn = booking.CheckIn;
        existingBooking.CheckOut = booking.CheckOut;

        _context.Entry(existingBooking).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
        {
            return NotFound();
        }

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("checkroom/{roomId}")]
    public async Task<ActionResult<bool>> CheckRoomAvailability(int roomId)
    {
        // Query the database to check if the room is booked
        var isRoomBooked = await _context.Bookings
            .AnyAsync(booking => booking.RoomId == roomId && booking.CheckOut > DateTime.Now);


        return Ok(isRoomBooked);
    }
}
