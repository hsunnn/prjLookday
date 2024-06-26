﻿namespace prjLookday.ViewModels
{
    public class CActivityModelViewModel
    {
        public int ActivityId { get; set; }
        public int ModelId { get; set; }
        public string ModelName { get; set; }
        public decimal? ModelPrice { get; set; }
        public DateOnly? ModelDate { get; set; }
        public string ModelContent { get; set; }
    }
}
