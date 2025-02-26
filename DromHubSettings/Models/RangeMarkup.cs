using System;

namespace DromHubSettings.Models
{
    /// <summary>
    /// Модель диапазона цен, содержащая минимальную и максимальную цену, а также наценку (коэффициент).
    /// </summary>
    public class RangeMarkup
    {
        /// <summary>
        /// Уникальный идентификатор диапазона (ID генерируется программно).
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Минимальная цена диапазона.
        /// </summary>
        public double MinPrice { get; set; }

        /// <summary>
        /// Максимальная цена диапазона.
        /// </summary>
        public double MaxPrice { get; set; }

        /// <summary>
        /// Наценка (коэффициент) для данного диапазона цен.
        /// </summary>
        public double Markup { get; set; }
    }
}
