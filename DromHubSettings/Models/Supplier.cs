using System;

namespace DromHubSettings.Models
{
    /// <summary>
    /// Модель поставщика.
    /// Содержит информацию о поставщике, включая имя, адрес электронной почты, идентификатор локальности и статус активности.
    /// </summary>
    public class Supplier
    {
        /// <summary>
        /// Уникальный идентификатор поставщика (генерируется программно).
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название поставщика.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Адрес электронной почты поставщика.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Идентификатор локальности (хранится в базе).
        /// </summary>
        public Guid LocalityId { get; set; }

        /// <summary>
        /// Порядковый номер или индекс поставщика.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Статус активности поставщика (по умолчанию активен).
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Локальность поставщика (получается через JOIN, не хранится в таблице).
        /// </summary>
        public string LocalityName { get; set; }

        /// <summary>
        /// Свойство для группировки поставщиков по первой букве названия.
        /// Если имя отсутствует, используется символ "#".
        /// </summary>
        public string GroupKey => !string.IsNullOrEmpty(Name)
            ? Name.Substring(0, 1).ToUpperInvariant()
            : "#";
    }
}
