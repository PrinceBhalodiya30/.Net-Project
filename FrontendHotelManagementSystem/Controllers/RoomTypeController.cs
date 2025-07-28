using FrontendHotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace FrontendHotelManagementSystem.Controllers
{
    public class RoomTypeController : Controller
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoomTypeController> _logger;

        public RoomTypeController(IHttpClientFactory httpClientFactory, ILogger<RoomTypeController> logger)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7077/api/");
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _client.GetAsync("RoomType");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<RoomType>>(json);
                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching room types.");
                TempData["Error"] = "Unable to load room types.";
                return View(new List<RoomType>());
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _client.DeleteAsync($"RoomType/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting room type ID {id}");
                TempData["Error"] = "Unable to delete room type.";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            try
            {
                RoomType type = new RoomType();

                if (id != null)
                {
                    var response = await _client.GetAsync($"RoomType/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "Room type not found.";
                        return RedirectToAction("Index");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    type = JsonConvert.DeserializeObject<RoomType>(json);
                }

                return View(type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading room type form for ID {id}.");
                TempData["Error"] = "Unable to load form.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(RoomType type)
        {
            if (!ModelState.IsValid)
                return View(type);

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(type), Encoding.UTF8, "application/json");

                if (type.RoomTypeId == 0)
                    await _client.PostAsync("RoomType", content);
                else
                    await _client.PutAsync($"RoomType/{type.RoomTypeId}", content);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving room type ID {type.RoomTypeId}");
                TempData["Error"] = "Unable to save room type.";
                return View(type);
            }
        }
    }
}
