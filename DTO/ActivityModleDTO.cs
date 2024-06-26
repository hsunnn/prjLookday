namespace prjLookday.DTO
{
    public class ActivityModleDTO
    {
        public int? ActivityId { get; set; }

        public string ModelName { get; set; }

        public decimal? ModelPrice { get; set; }

        public DateOnly? ModelDate { get; set; }

        public string ModelContent { get; set; }
    }
}
