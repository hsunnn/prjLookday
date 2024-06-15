namespace prjLookday.ViewModels
{
    public class COrderListViewModel
    {
        public int BookingId { get; set; }
        public DateTime? BookingDate { get; set; }
        public decimal Price { get; set; }

        public string? UserName { get; set; }

        public string? ActivityName { get; set; }
        public string? bookingStatus { get; set; }
        public int? member { get; set; }

        public string? remaining { get; set; }
    }
}
