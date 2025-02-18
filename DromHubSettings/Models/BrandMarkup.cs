using System;

namespace DromHubSettings.Models
{
    public class BrandMarkup
    {
        public Guid Id { get; set; } // уникальный идентификатор, генерируется в программе
        public string BrandName { get; set; }
        public double Markup { get; set; }

        // Свойство для группировки — первая буква названия бренда
        public string GroupKey => !string.IsNullOrEmpty(BrandName)
            ? BrandName.Substring(0, 1).ToUpperInvariant()
            : "#";
    }
}
