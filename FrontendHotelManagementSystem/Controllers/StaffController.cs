using FrontendHotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace FrontendHotelManagementSystem.Controllers
{
    public class StaffController : Controller
    {
        private readonly HttpClient _client;
        private readonly ILogger<StaffController> _logger;

        public StaffController(IHttpClientFactory httpClientFactory, ILogger<StaffController> logger)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7077/api/");
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _client.GetAsync("Staff");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<Staff>>(json);
                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching staff.");
                TempData["Error"] = "Unable to load staff.";
                return View(new List<Staff>());
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _client.DeleteAsync($"Staff/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting staff ID {id}");
                TempData["Error"] = "Unable to delete staff.";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            try
            {
                Staff staff = new Staff();

                if (id != null)
                {
                    var response = await _client.GetAsync($"Staff/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "Staff not found.";
                        return RedirectToAction("Index");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    staff = JsonConvert.DeserializeObject<Staff>(json);
                }

                return View(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading staff form for ID {id}.");
                TempData["Error"] = "Unable to load form.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(Staff staff)
        {
            if (!ModelState.IsValid)
                return View(staff);

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(staff), Encoding.UTF8, "application/json");

                if (staff.StaffId == 0)
                    await _client.PostAsync("Staff", content);
                else
                    await _client.PutAsync($"Staff/{staff.StaffId}", content);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving staff ID {staff.StaffId}");
                TempData["Error"] = "Unable to save staff.";
                return View(staff);
            }
        }
    }
}
