using ClientHotelSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ClientHotelSystem.Controllers
{
    public class RoomsController : Controller
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(IHttpClientFactory httpClientFactory, ILogger<RoomsController> logger)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7077/api/");
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? roomType, decimal? minPrice, decimal? maxPrice, int? capacity)
        {
            try
            {
                // Get all available rooms
                var roomsResponse = await _client.GetAsync("Room");
                var roomsJson = await roomsResponse.Content.ReadAsStringAsync();
                var rooms = JsonConvert.DeserializeObject<List<Room>>(roomsJson) ?? new List<Room>();

                // Get room types
                var roomTypesResponse = await _client.GetAsync("RoomType");
                var roomTypesJson = await roomTypesResponse.Content.ReadAsStringAsync();
                var roomTypes = JsonConvert.DeserializeObject<List<RoomType>>(roomTypesJson) ?? new List<RoomType>();

                // Map room type names
                foreach (var room in rooms)
                {
                    var type = roomTypes.FirstOrDefault(rt => rt.RoomTypeId == room.RoomTypeId);
                    room.RoomTypeName = type?.RoomTypeName;
                }

                // Filter available rooms
                var availableRooms = rooms.Where(r => r.IsAvaliable).ToList();

                // Apply filters
                if (!string.IsNullOrEmpty(roomType))
                {
                    availableRooms = availableRooms.Where(r => r.RoomTypeName?.Contains(roomType, StringComparison.OrdinalIgnoreCase) == true).ToList();
                }

                if (minPrice.HasValue)
                {
                    availableRooms = availableRooms.Where(r => r.PricePerNight >= minPrice.Value).ToList();
                }

                if (maxPrice.HasValue)
                {
                    availableRooms = availableRooms.Where(r => r.PricePerNight <= maxPrice.Value).ToList();
                }

                if (capacity.HasValue)
                {
                    availableRooms = availableRooms.Where(r => r.Capacity >= capacity.Value).ToList();
                }

                ViewBag.RoomTypes = roomTypes;
                ViewBag.CurrentFilters = new { roomType, minPrice, maxPrice, capacity };

                return View(availableRooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading rooms");
                TempData["Error"] = "Unable to load rooms. Please try again.";
                return View(new List<Room>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _client.GetAsync($"Room/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Room not found.";
                    return RedirectToAction("Index");
                }

                var json = await response.Content.ReadAsStringAsync();
                var room = JsonConvert.DeserializeObject<Room>(json);

                // Get room type name
                var roomTypesResponse = await _client.GetAsync("RoomType");
                var roomTypesJson = await roomTypesResponse.Content.ReadAsStringAsync();
                var roomTypes = JsonConvert.DeserializeObject<List<RoomType>>(roomTypesJson) ?? new List<RoomType>();
                
                var roomType = roomTypes.FirstOrDefault(rt => rt.RoomTypeId == room.RoomTypeId);
                room.RoomTypeName = roomType?.RoomTypeName;

                return View(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading room details for ID {id}");
                TempData["Error"] = "Unable to load room details.";
                return RedirectToAction("Index");
            }
        }
    }
}