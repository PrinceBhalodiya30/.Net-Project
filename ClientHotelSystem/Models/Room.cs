using System.ComponentModel.DataAnnotations;

namespace ClientHotelSystem.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public int RoomTypeId { get; set; }
        public int Capacity { get; set; }
        public decimal? PricePerNight { get; set; }
        public bool IsAvaliable { get; set; }
        public string? RoomTypeName { get; set; }
    }
}