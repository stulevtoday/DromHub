using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DromHubSettings.Models
{
    public class SupplierExcelMapping
    {
        /// <summary>
        /// Идентификатор сопоставления.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор настроек импорта, к которым принадлежит данное сопоставление.
        /// </summary>
        public Guid SupplierExcelSettingsId { get; set; }

        /// <summary>
        /// Идентификатор свойства Excel (например, "ProductName", "Brand" и т.д.).
        /// Обычно ссылается на запись в справочнике Excel-свойств.
        /// </summary>
        public Guid ExcelMappingId { get; set; }
        public string ExcelMappingName { get; set; }

        /// <summary>
        /// Номер столбца в Excel файле, из которого берётся значение.
        /// </summary>
        public int ColumnIndex { get; set; }
    }
}
