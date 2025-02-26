using System;

namespace DromHubSettings.Models
{
    /// <summary>
    /// Модель наценки поставщика.
    /// Содержит идентификатор поставщика, значение наценки и имя поставщика.
    /// Имя используется для группировки наценок по первой букве.
    /// </summary>
    public class SupplierMarkup
    {
        /// <summary>
        /// Идентификатор поставщика.
        /// </summary>
        public Guid SupplierId { get; set; }

        /// <summary>
        /// Коэффициент наценки для данного поставщика.
        /// </summary>
        public double Markup { get; set; }

        /// <summary>
        /// Имя поставщика (получается через JOIN, не сохраняется в таблице).
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// Свойство для группировки наценок по первой букве имени поставщика.
        /// Если имя отсутствует, используется символ "#".
        /// </summary>
        public string GroupKey => !string.IsNullOrEmpty(SupplierName)
            ? SupplierName.Substring(0, 1).ToUpperInvariant()
            : "#";
    }
}
