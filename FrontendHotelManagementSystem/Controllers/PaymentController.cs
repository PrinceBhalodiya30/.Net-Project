using FrontendHotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace FrontendHotelManagementSystem.Controllers
{
    public class PaymentController : Controller
    {
        private readonly HttpClient _client;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IHttpClientFactory httpClientFactory, ILogger<PaymentController> logger)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7077/api/");
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _client.GetAsync("Payment");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<Payment>>(json);
                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payments.");
                TempData["Error"] = "Unable to load payments.";
                return View(new List<Payment>());
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _client.DeleteAsync($"Payment/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting payment ID {id}");
                TempData["Error"] = "Unable to delete payment.";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            _logger.LogInformation($"AddEdit GET called with ID: {id}");
            try
            {
                Payment payment = new Payment();

                if (id != null)
                {
                    _logger.LogInformation($"Loading existing payment with ID: {id}");
                    var response = await _client.GetAsync($"Payment/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning($"Payment not found for ID: {id}");
                        TempData["Error"] = "Payment not found.";
                        return RedirectToAction("Index");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    payment = JsonConvert.DeserializeObject<Payment>(json);
                    _logger.LogInformation($"Loaded payment: {JsonConvert.SerializeObject(payment)}");
                }
                else
                {
                    _logger.LogInformation("Creating new payment form");
                }

                // Fetch bookings for dropdown
                await PopulateBookingsDropdown();

                return View(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading payment form for ID {id}.");
                TempData["Error"] = $"Unable to load form: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(Payment payment)
        {
            _logger.LogInformation($"AddEdit POST called with PaymentId: {payment.PaymentId}, BookingId: {payment.BookingId}");
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid. Errors: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                // Repopulate dropdown data when model is invalid
                await PopulateBookingsDropdown();
                return View(payment);
            }

            try
            {
                var jsonContent = JsonConvert.SerializeObject(payment);
                _logger.LogInformation($"Sending JSON: {jsonContent}");
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                if (payment.PaymentId == 0)
                {
                    _logger.LogInformation("Creating new payment");
                    response = await _client.PostAsync("Payment", content);
                }
                else
                {
                    _logger.LogInformation($"Updating payment {payment.PaymentId}");
                    response = await _client.PutAsync($"Payment/{payment.PaymentId}", content);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"API returned error: {response.StatusCode} - {errorContent}");
                    TempData["Error"] = $"API Error: {response.StatusCode} - {errorContent}";
                    await PopulateBookingsDropdown();
                    return View(payment);
                }

                _logger.LogInformation("Payment saved successfully");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving payment ID {payment.PaymentId}");
                TempData["Error"] = $"Unable to save payment: {ex.Message}";
                // Repopulate dropdown data when there's an error
                await PopulateBookingsDropdown();
                return View(payment);
            }
        }

        private async Task PopulateBookingsDropdown()
        {
            _logger.LogInformation("Populating bookings dropdown");
            try
            {
                var bookingsResponse = await _client.GetAsync("Booking");
                _logger.LogInformation($"Bookings API response status: {bookingsResponse.StatusCode}");
                
                if (bookingsResponse.IsSuccessStatusCode)
                {
                    var bookingsJson = await bookingsResponse.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Bookings JSON: {bookingsJson}");
                    var bookings = JsonConvert.DeserializeObject<List<Booking>>(bookingsJson);
                    ViewBag.Bookings = bookings?.Select(b => new SelectListItem
                    {
                        Value = b.BookingId.ToString(),
                        Text = $"Booking #{b.BookingId} - Customer: {b.CustomerId} - Room: {b.RoomId}"
                    }).ToList();
                    _logger.LogInformation($"Populated {ViewBag.Bookings?.Count ?? 0} booking options");
                }
                else
                {
                    var errorContent = await bookingsResponse.Content.ReadAsStringAsync();
                    _logger.LogWarning($"Bookings API returned error: {bookingsResponse.StatusCode} - {errorContent}");
                    ViewBag.Bookings = new List<SelectListItem>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bookings for dropdown.");
                ViewBag.Bookings = new List<SelectListItem>();
            }
        }
    }
}
