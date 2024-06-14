namespace prjLookday.ViewModels
{
    public class CBookingListViewModel
    {
        public int BookingId { get; set; }
        public DateTime? BookingDate { get; set; }
        public string UserName { get; set; }
        public string ActivityName { get; set; }
        public string Price { get; set; }
        public string bookingStatus { get; set; }
        public int? member { get; set; }
    }
}
