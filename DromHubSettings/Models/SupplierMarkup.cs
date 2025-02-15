using System;

namespace DromHubSettings.Models
{
    public class SupplierMarkup
    {
        public Guid SupplierId { get; set; }
        public double Markup { get; set; }
        // Это поле не хранится в таблице supplier_markups, но получаем через JOIN
        public string SupplierName { get; set; }
    }
}
