using FrontendHotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace FrontendHotelManagementSystem.Controllers
{
    public class RoomController : Controller
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoomController> _logger;

        public RoomController(IHttpClientFactory httpClientFactory, ILogger<RoomController> logger)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7077/api/");
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _client.GetAsync("Room");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<Room>>(json);
                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rooms.");
                TempData["Error"] = "Unable to load rooms.";
                return View(new List<Room>());
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _client.DeleteAsync($"Room/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting room ID {id}");
                TempData["Error"] = "Unable to delete room.";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            try
            {
                Room room = new Room();

                if (id != null)
                {
                    var response = await _client.GetAsync($"Room/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "Room not found.";
                        return RedirectToAction("Index");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    room = JsonConvert.DeserializeObject<Room>(json);
                }

                // Fetch room types for dropdown
                await PopulateRoomTypesDropdown();

                return View(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading room form for ID {id}.");
                TempData["Error"] = "Unable to load form.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(Room room)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdown data when model is invalid
                await PopulateRoomTypesDropdown();
                return View(room);
            }

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(room), Encoding.UTF8, "application/json");

                if (room.RoomId == 0)
                    await _client.PostAsync("Room", content);
                else
                    await _client.PutAsync($"Room/{room.RoomId}", content);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving room ID {room.RoomId}");
                TempData["Error"] = "Unable to save room.";
                // Repopulate dropdown data when there's an error
                await PopulateRoomTypesDropdown();
                return View(room);
            }
        }

        private async Task PopulateRoomTypesDropdown()
        {
            try
            {
                var roomTypesResponse = await _client.GetAsync("RoomType");
                if (roomTypesResponse.IsSuccessStatusCode)
                {
                    var roomTypesJson = await roomTypesResponse.Content.ReadAsStringAsync();
                    var roomTypes = JsonConvert.DeserializeObject<List<RoomType>>(roomTypesJson);
                    ViewBag.RoomTypes = roomTypes?.Select(rt => new SelectListItem
                    {
                        Value = rt.RoomTypeId.ToString(),
                        Text = rt.RoomTypeName
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching room types for dropdown.");
                ViewBag.RoomTypes = new List<SelectListItem>();
            }
        }
    }
}
