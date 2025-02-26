using System;

namespace DromHubSettings.Models
{
    /// <summary>
    /// Модель локальности.
    /// Содержит уникальный идентификатор, название, срок доставки и адрес электронной почты для экспорта.
    /// </summary>
    public class Locality
    {
        /// <summary>
        /// Уникальный идентификатор локальности.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название локальности.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Срок доставки в днях.
        /// </summary>
        public int DeliveryTime { get; set; }

        /// <summary>
        /// Адрес электронной почты для экспорта файлов.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Свойство для группировки локальностей по первой букве названия.
        /// Если название отсутствует, возвращается "#".
        /// </summary>
        public string GroupKey => !string.IsNullOrEmpty(Name)
            ? Name.Substring(0, 1).ToUpperInvariant()
            : "#";
    }
}
