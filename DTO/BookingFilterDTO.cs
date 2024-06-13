namespace prjLookday.DTO
{
    public class BookingFilterDTO
    {
        public int BookingId { get; set; }

        public DateTime? BookingDate { get; set; }
       
        public string? ActivityName { get; set; }
        public DateOnly? ActivityDate { get; set; }
        public string? bookingStatus { get; set; }

  
    }
}
