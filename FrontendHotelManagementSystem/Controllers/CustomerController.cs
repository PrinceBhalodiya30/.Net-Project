using FrontendHotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace FrontendHotelManagementSystem.Controllers
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

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _client.GetAsync("Customer");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<Customer>>(json);
                return View(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customers.");
                TempData["Error"] = "Unable to load customers.";
                return View(new List<Customer>());
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _client.DeleteAsync($"Customer/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting customer with ID {id}.");
                TempData["Error"] = "Unable to delete customer.";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            try
            {
                Customer customer = new Customer();

                if (id != null)
                {
                    var response = await _client.GetAsync($"Customer/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "Customer not found.";
                        return RedirectToAction("Index");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    customer = JsonConvert.DeserializeObject<Customer>(json);
                }

                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading customer form for ID {id}.");
                TempData["Error"] = "Unable to load form.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");

                if (customer.CustomerId == 0)
                {
                    var response = await _client.PostAsync("Customer", content);
                    response.EnsureSuccessStatusCode();
                }
                else
                {
                    var response = await _client.PutAsync($"Customer/{customer.CustomerId}", content);
                    response.EnsureSuccessStatusCode();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving customer with ID {customer.CustomerId}.");
                TempData["Error"] = "Unable to save customer.";
                return View(customer);
            }
        }
    }
}
