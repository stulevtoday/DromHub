using System;

namespace DromHubSettings.Models
{
    /// <summary>
    /// Модель наценки по бренду.
    /// Содержит идентификатор, название бренда и значение наценки.
    /// </summary>
    public class BrandMarkup
    {
        /// <summary>
        /// Уникальный идентификатор наценки (генерируется программно).
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название бренда.
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// Значение наценки (коэффициент).
        /// </summary>
        public double Markup { get; set; }

        /// <summary>
        /// Свойство для группировки наценок по первой букве названия бренда.
        /// Если имя отсутствует, используется символ "#".
        /// </summary>
        public string GroupKey => !string.IsNullOrEmpty(BrandName)
            ? BrandName.Substring(0, 1).ToUpperInvariant()
            : "#";
    }
}
