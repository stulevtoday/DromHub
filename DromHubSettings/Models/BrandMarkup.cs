using System;

namespace DromHubSettings.Models
{
    public class BrandMarkup
    {
        public Guid Id { get; set; } // уникальный идентификатор, генерируется в программе
        public string BrandName { get; set; }
        public double Markup { get; set; }
    }
}
