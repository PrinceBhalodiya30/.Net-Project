using ClientHotelSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace ClientHotelSystem.Controllers
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

        public async Task<IActionResult> Create(int? roomId)
        {
            try
            {
                var booking = new Booking();
                if (roomId.HasValue)
                {
                    booking.RoomId = roomId.Value;
                    
                    // Get room details for display
                    var roomResponse = await _client.GetAsync($"Room/{roomId}");
                    if (roomResponse.IsSuccessStatusCode)
                    {
                        var roomJson = await roomResponse.Content.ReadAsStringAsync();
                        var room = JsonConvert.DeserializeObject<Room>(roomJson);
                        ViewBag.SelectedRoom = room;
                    }
                }

                await PopulateDropdowns();
                return View(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading booking form");
                TempData["Error"] = "Unable to load booking form.";
                return RedirectToAction("Index", "Rooms");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(booking);
            }

            try
            {
                // Set default booking status
                booking.BookingStatus = "Pending";
                
                var content = new StringContent(JsonConvert.SerializeObject(booking), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("Booking", content);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Your booking has been submitted successfully! We will contact you soon.";
                    return RedirectToAction("Confirmation", new { id = booking.BookingId });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = "Unable to submit booking. Please try again.";
                    await PopulateDropdowns();
                    return View(booking);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                TempData["Error"] = "Unable to submit booking. Please try again.";
                await PopulateDropdowns();
                return View(booking);
            }
        }

        public IActionResult Confirmation()
        {
            return View();
        }

        public async Task<IActionResult> MyBookings(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return View("SearchBookings");
            }

            try
            {
                // Get customer by email
                var customersResponse = await _client.GetAsync("Customer");
                var customersJson = await customersResponse.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(customersJson) ?? new List<Customer>();
                
                var customer = customers.FirstOrDefault(c => c.Email?.Equals(email, StringComparison.OrdinalIgnoreCase) == true);
                
                if (customer == null)
                {
                    TempData["Error"] = "No customer found with this email address.";
                    return View("SearchBookings");
                }

                // Get bookings for this customer
                var bookingsResponse = await _client.GetAsync("Booking");
                var bookingsJson = await bookingsResponse.Content.ReadAsStringAsync();
                var allBookings = JsonConvert.DeserializeObject<List<Booking>>(bookingsJson) ?? new List<Booking>();
                
                var customerBookings = allBookings.Where(b => b.CustomerId == customer.CustomerId).ToList();

                // Get rooms and room types for display
                var roomsResponse = await _client.GetAsync("Room");
                var roomsJson = await roomsResponse.Content.ReadAsStringAsync();
                var rooms = JsonConvert.DeserializeObject<List<Room>>(roomsJson) ?? new List<Room>();

                // Map room details to bookings
                foreach (var booking in customerBookings)
                {
                    var room = rooms.FirstOrDefault(r => r.RoomId == booking.RoomId);
                    if (room != null)
                    {
                        booking.RoomNumber = room.RoomNumber;
                        booking.RoomPrice = room.PricePerNight;
                    }
                    booking.CustomerName = customer.FullName;
                }

                ViewBag.Customer = customer;
                return View(customerBookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading bookings for email {email}");
                TempData["Error"] = "Unable to load bookings. Please try again.";
                return View("SearchBookings");
            }
        }

        public IActionResult SearchBookings()
        {
            return View();
        }

        private async Task PopulateDropdowns()
        {
            // Get available rooms
            try
            {
                var roomsResponse = await _client.GetAsync("Room");
                if (roomsResponse.IsSuccessStatusCode)
                {
                    var roomsJson = await roomsResponse.Content.ReadAsStringAsync();
                    var rooms = JsonConvert.DeserializeObject<List<Room>>(roomsJson) ?? new List<Room>();
                    
                    // Get room types
                    var roomTypesResponse = await _client.GetAsync("RoomType");
                    var roomTypesJson = await roomTypesResponse.Content.ReadAsStringAsync();
                    var roomTypes = JsonConvert.DeserializeObject<List<RoomType>>(roomTypesJson) ?? new List<RoomType>();

                    var availableRooms = rooms.Where(r => r.IsAvaliable).Select(r => {
                        var roomType = roomTypes.FirstOrDefault(rt => rt.RoomTypeId == r.RoomTypeId);
                        return new SelectListItem
                        {
                            Value = r.RoomId.ToString(),
                            Text = $"{r.RoomNumber} - {roomType?.RoomTypeName} - {r.PricePerNight:C}/night"
                        };
                    }).ToList();

                    ViewBag.Rooms = availableRooms;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rooms for dropdown");
                ViewBag.Rooms = new List<SelectListItem>();
            }
        }
    }
}