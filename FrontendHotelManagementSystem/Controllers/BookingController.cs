using FrontendHotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace FrontendHotelManagementSystem.Controllers
{
    public class BookingController : Controller
    {
        private readonly HttpClient _client;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IHttpClientFactory httpClientFactory, ILogger<BookingController> logger)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7077/api/");
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _client.GetAsync("Booking");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<Booking>>(json);
                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bookings.");
                TempData["Error"] = "Unable to load bookings.";
                return View(new List<Booking>());
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _client.DeleteAsync($"Booking/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting booking with ID {id}.");
                TempData["Error"] = "Unable to delete booking.";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            try
            {
                Booking booking = new Booking();

                if (id != null)
                {
                    var response = await _client.GetAsync($"Booking/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "Booking not found.";
                        return RedirectToAction("Index");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    booking = JsonConvert.DeserializeObject<Booking>(json);
                }

                // Fetch dropdown data
                await PopulateDropdowns();

                return View(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading booking form for ID {id}.");
                TempData["Error"] = "Unable to load form.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdown data when model is invalid
                await PopulateDropdowns();
                return View(booking);
            }

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(booking), Encoding.UTF8, "application/json");

                if (booking.BookingId == 0)
                {
                    var response = await _client.PostAsync("Booking", content);
                    response.EnsureSuccessStatusCode();
                }
                else
                {
                    var response = await _client.PutAsync($"Booking/{booking.BookingId}", content);
                    response.EnsureSuccessStatusCode();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving booking with ID {booking.BookingId}.");
                TempData["Error"] = "Unable to save booking.";
                // Repopulate dropdown data when there's an error
                await PopulateDropdowns();
                return View(booking);
            }
        }

        private async Task PopulateDropdowns()
        {
            // Fetch customers for dropdown
            try
            {
                var customersResponse = await _client.GetAsync("Customer");
                if (customersResponse.IsSuccessStatusCode)
                {
                    var customersJson = await customersResponse.Content.ReadAsStringAsync();
                    var customers = JsonConvert.DeserializeObject<List<Customer>>(customersJson);
                    ViewBag.Customers = customers?.Select(c => new SelectListItem
                    {
                        Value = c.CustomerId.ToString(),
                        Text = $"{c.FullName} - {c.Email}"
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customers for dropdown.");
                ViewBag.Customers = new List<SelectListItem>();
            }

            // Fetch rooms for dropdown
            try
            {
                var roomsResponse = await _client.GetAsync("Room");
                if (roomsResponse.IsSuccessStatusCode)
                {
                    var roomsJson = await roomsResponse.Content.ReadAsStringAsync();
                    var rooms = JsonConvert.DeserializeObject<List<Room>>(roomsJson);
                    ViewBag.Rooms = rooms?.Select(r => new SelectListItem
                    {
                        Value = r.RoomId.ToString(),
                        Text = $"{r.RoomNumber} - {r.PricePerNight:C}/night"
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rooms for dropdown.");
                ViewBag.Rooms = new List<SelectListItem>();
            }
        }
    }
}
