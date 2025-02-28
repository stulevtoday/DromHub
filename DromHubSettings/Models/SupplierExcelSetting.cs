using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DromHubSettings.Models
{
    /// <summary>
    /// Модель настроек импорта Excel файла для поставщика.
    /// Содержит общие настройки, такие как начальная строка, имя листа и т.д.
    /// </summary>
    public class SupplierExcelSetting
    {
        /// <summary>
        /// Идентификатор настроек импорта.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор поставщика.
        /// </summary>
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }

        /// <summary>
        /// Номер строки, с которой начинается импорт данных из Excel.
        /// </summary>
        public int InitialRow { get; set; }

        /// <summary>
        /// Коллекция сопоставлений столбцов для этих настроек.
        /// При удалении настроек (SupplierExcelSettings) с каскадным удалением будут удалены все связанные сопоставления.
        /// </summary>
        public ICollection<SupplierExcelMapping> Mappings { get; set; }

        /// <summary>
        /// Свойство для группировки сопоставлений.
        /// Например, можно группировать по первой букве имени поставщика.
        /// </summary>
        public string GroupKey => !string.IsNullOrEmpty(SupplierName)
            ? SupplierName.Substring(0, 1).ToUpperInvariant()
            : "#";
    }
}
