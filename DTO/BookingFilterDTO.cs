namespace prjLookday.DTO
{
    public class BookingFilterDTO
    {
        public int BookingId { get; set; }

        public DateTime? BookingDate { get; set; }
       
        public string? ActivityName { get; set; }
        public DateTime? ActivityDate { get; set; }
        public string? bookingStatus { get; set; }

  
    }
}
