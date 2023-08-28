using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RoomService.Data;

namespace RoomService.Controllers;
[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    public RoomsController(AppDbContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
    {
        return await _context.Rooms.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> GetRoom(int id)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
        {
            return NotFound();
        }

        return room;
    }

    [HttpPost]
    public async Task<ActionResult<Room>> CreateRoom(Room room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRoom), new { id = room.RoomId }, room);
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRoom(int id)
    {
        // Check with the Booking Service to see if the room is booked
        var isRoomBooked = await CheckRoomAvailabilityWithBookingService(id);

        if (isRoomBooked)
        {
            return BadRequest("Cannot delete the room because it is currently booked.");
        }

        var roomToDelete = await _context.Rooms.FindAsync(id);

        if (roomToDelete == null)
        {
            return NotFound();
        }

        _context.Rooms.Remove(roomToDelete);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRoom(int id, Room room)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingRoom = await _context.Rooms.FindAsync(id);

        if (existingRoom == null)
        {
            return NotFound();
        }

        var bookingStatus = await CheckRoomAvailabilityWithBookingService(id);
        if (bookingStatus)
        {
            ModelState.AddModelError("", "The room is booked and the booking is not done. Cannot update.");
            return BadRequest(ModelState);
        }

        existingRoom.RoomType = room.RoomType;
        existingRoom.Capacity = room.Capacity;

        _context.Rooms.Update(existingRoom);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> CheckRoomAvailabilityWithBookingService(int roomId)
    {

        var httpClient = _httpClientFactory.CreateClient();



        string serviceUrl = _configuration["ServiceUrl"];
        var response = await httpClient.GetAsync($"{serviceUrl}/api/bookings/checkroom/{roomId}");
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode && bool.TryParse(content, out var isRoomBooked))
        {
            return isRoomBooked;
        }

        return true; // Default to preventing deletion if there's an issue with the Booking Service
    }
}
