using ClientHotelSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace ClientHotelSystem.Controllers
{
    public class CustomerController : Controller
    {
        private readonly HttpClient _client;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(IHttpClientFactory httpClientFactory, ILogger<CustomerController> logger)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7077/api/");
            _logger = logger;
        }

        public IActionResult Register()
        {
            return View(new Customer());
        }

        [HttpPost]
        public async Task<IActionResult> Register(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("Customer", content);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Registration successful! You can now make bookings.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Error"] = "Registration failed. Please try again.";
                    return View(customer);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering customer");
                TempData["Error"] = "Registration failed. Please try again.";
                return View(customer);
            }
        }
    }
}