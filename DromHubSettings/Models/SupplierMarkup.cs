using System;

namespace DromHubSettings.Models
{
    public class SupplierMarkup
    {
        public Guid Id { get; set; }
        public string SupplierName { get; set; }
        public double Markup { get; set; } = 125; // базовое значение 125%
    }
}
