using System.Linq;

namespace RoomService.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            if (!context.Rooms.Any())
            {
                context.Rooms.AddRange(
                    new Room { RoomType = "Single", Capacity = 1 },
                    new Room { RoomType = "Double", Capacity = 2 },
                    new Room { RoomType = "Suite", Capacity = 4 }
                );

                context.SaveChanges();
            }
        }
    }

}