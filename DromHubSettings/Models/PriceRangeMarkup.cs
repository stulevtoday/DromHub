using System;

namespace DromHubSettings.Models
{
    public class PriceRangeMarkup
    {
        public Guid Id { get; set; } // теперь ID генерируется в программе
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public double Markup { get; set; }
    }
}
