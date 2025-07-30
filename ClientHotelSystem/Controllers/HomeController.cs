using System.Diagnostics;
using ClientHotelSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ClientHotelSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _client;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7077/api/");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get featured rooms (available rooms)
                var roomsResponse = await _client.GetAsync("Room");
                var roomsJson = await roomsResponse.Content.ReadAsStringAsync();
                var rooms = JsonConvert.DeserializeObject<List<Room>>(roomsJson) ?? new List<Room>();
                
                // Get room types for display
                var roomTypesResponse = await _client.GetAsync("RoomType");
                var roomTypesJson = await roomTypesResponse.Content.ReadAsStringAsync();
                var roomTypes = JsonConvert.DeserializeObject<List<RoomType>>(roomTypesJson) ?? new List<RoomType>();

                // Map room type names to rooms
                foreach (var room in rooms)
                {
                    var roomType = roomTypes.FirstOrDefault(rt => rt.RoomTypeId == room.RoomTypeId);
                    room.RoomTypeName = roomType?.RoomTypeName;
                }

                ViewBag.FeaturedRooms = rooms.Where(r => r.IsAvaliable).Take(6).ToList();
                ViewBag.TotalRooms = rooms.Count;
                ViewBag.AvailableRooms = rooms.Count(r => r.IsAvaliable);
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page data");
                ViewBag.FeaturedRooms = new List<Room>();
                ViewBag.TotalRooms = 0;
                ViewBag.AvailableRooms = 0;
                return View();
            }
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}