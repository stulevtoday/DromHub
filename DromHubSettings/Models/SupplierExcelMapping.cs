using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DromHubSettings.Models
{
    public class SupplierExcelMapping
    {
        public Guid SupplierId { get; set; }         // Ссылка на поставщика
        public int ColumnProductName { get; set; }     // Номер столбца для названия товара
        public int ColumnBrand { get; set; }           // Номер столбца для бренда
        public int ColumnCatalogName { get; set; }     // Номер столбца для каталожного наименования
        public int ColumnQuantity { get; set; }        // Номер столбца для количества
        public int ColumnPrice { get; set; }           // Номер столбца для цены
        public int ColumnMultiple { get; set; }         // Номер столбца для множества (опционально)

        // Это свойство не сохраняется в таблице, а используется только для отображения
        public string SupplierName { get; set; }
    }

}
