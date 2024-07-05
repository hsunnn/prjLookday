namespace prjLookdayOrder.DTO
{
    public class BookingDTO
    {
       
        public int BookingId { get; set; }
        public DateTime? BookingDate { get; set; } 
        public decimal Price { get; set; }

        public string userDisplay { get; set; }

        public int UserID { get; set; }

        public string? UserName { get; set; }

        public string activityDisplay { get; set; }

        public int ActivityID { get; set; }
     
        public string? ActivityName { get; set; } 
        public string? bookingStatus { get; set; }
        public int? member { get; set; }

        public string? remaining { get; set; }
    }
}
