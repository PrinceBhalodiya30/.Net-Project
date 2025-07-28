namespace FrontendHotelManagementSystem.Models
{
    public class DashboardModel
    {
        public int TotalBookings { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int TotalStaff { get; set; }
        public int TotalPayments { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingRequests { get; set; }
        public List<Booking> RecentBookings { get; set; } = new();
        public List<Customer> RecentCustomers { get; set; } = new();
        public List<Room> AvailableRoomList { get; set; } = new();
        public List<Payment> RecentPayments { get; set; } = new();
        public List<Staff> RecentStaff { get; set; } = new();
    }
}