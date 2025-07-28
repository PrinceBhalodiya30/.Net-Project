using System.Diagnostics;
using FrontendHotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FrontendHotelManagementSystem.Controllers
{
    public class DashboardViewModel
    {
        public int TotalBookings { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int TotalStaff { get; set; }
        public int TotalPayments { get; set; }
        public List<Booking> RecentBookings { get; set; } = new();
        public List<Customer> RecentCustomers { get; set; } = new();
        public List<Room> AvailableRoomList { get; set; } = new();
        public List<Payment> RecentPayments { get; set; } = new();
        public List<Staff> RecentStaff { get; set; } = new();
    }

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _factory;
        private readonly string _apiBase = "https://localhost:7077/api/";

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel();
            var client = _factory.CreateClient();
            client.BaseAddress = new Uri(_apiBase);
            try
            {
                // Bookings
                var bookingsRes = await client.GetAsync("Booking");
                var bookingsJson = await bookingsRes.Content.ReadAsStringAsync();
                var bookings = JsonConvert.DeserializeObject<List<Booking>>(bookingsJson) ?? new List<Booking>();
                model.TotalBookings = bookings.Count;
                model.RecentBookings = bookings.OrderByDescending(b => b.BookingId).Take(5).ToList();

                // Customers
                var customersRes = await client.GetAsync("Customer");
                var customersJson = await customersRes.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(customersJson) ?? new List<Customer>();
                model.TotalCustomers = customers.Count;
                model.RecentCustomers = customers.OrderByDescending(c => c.CustomerId).Take(5).ToList();

                // Rooms
                var roomsRes = await client.GetAsync("Room");
                var roomsJson = await roomsRes.Content.ReadAsStringAsync();
                var rooms = JsonConvert.DeserializeObject<List<Room>>(roomsJson) ?? new List<Room>();
                model.TotalRooms = rooms.Count;
                model.AvailableRooms = rooms.Count(r => r.IsAvaliable == true);
                model.AvailableRoomList = rooms.Where(r => r.IsAvaliable == true).Take(5).ToList();

                // Staff
                var staffRes = await client.GetAsync("Staff");
                var staffJson = await staffRes.Content.ReadAsStringAsync();
                var staff = JsonConvert.DeserializeObject<List<Staff>>(staffJson) ?? new List<Staff>();
                model.TotalStaff = staff.Count;
                model.RecentStaff = staff.OrderByDescending(s => s.StaffId).Take(5).ToList();

                // Payments
                var paymentsRes = await client.GetAsync("Payment");
                var paymentsJson = await paymentsRes.Content.ReadAsStringAsync();
                var payments = JsonConvert.DeserializeObject<List<Payment>>(paymentsJson) ?? new List<Payment>();
                model.TotalPayments = payments.Count;
                model.RecentPayments = payments.OrderByDescending(p => p.PaymentId).Take(5).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load dashboard data");
            }
            return View(model);
        }

        public IActionResult Privacy()
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
