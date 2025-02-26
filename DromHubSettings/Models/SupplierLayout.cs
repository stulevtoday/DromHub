using System;

namespace DromHubSettings.Models
{
    /// <summary>
    /// Модель сопоставления столбцов Excel файла для поставщика.
    /// Определяет, какой номер столбца Excel соответствует определённому полю поставщика.
    /// </summary>
    public class SupplierLayout
    {
        /// <summary>
        /// Идентификатор поставщика (ссылка на поставщика).
        /// </summary>
        public Guid SupplierId { get; set; }

        /// <summary>
        /// Номер столбца для названия товара.
        /// </summary>
        public int ColumnProductName { get; set; }

        /// <summary>
        /// Номер столбца для бренда.
        /// </summary>
        public int ColumnBrand { get; set; }

        /// <summary>
        /// Номер столбца для каталожного наименования.
        /// </summary>
        public int ColumnCatalogName { get; set; }

        /// <summary>
        /// Номер столбца для количества.
        /// </summary>
        public int ColumnQuantity { get; set; }

        /// <summary>
        /// Номер столбца для цены.
        /// </summary>
        public int ColumnPrice { get; set; }

        /// <summary>
        /// Номер столбца для множества (опционально).
        /// </summary>
        public int ColumnMultiple { get; set; }

        /// <summary>
        /// Имя поставщика для отображения (не сохраняется в базе данных).
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// Свойство для группировки сопоставлений.
        /// Например, можно группировать по первой букве имени поставщика.
        /// </summary>
        public string GroupKey => !string.IsNullOrEmpty(SupplierName)
            ? SupplierName.Substring(0, 1).ToUpperInvariant()
            : "#";
    }
}
