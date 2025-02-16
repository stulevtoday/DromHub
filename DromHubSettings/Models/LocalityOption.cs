using System;

namespace DromHubSettings.Models
{
    public class LocalityOption
    {
        public Guid Id { get; set; }          // Уникальный идентификатор
        public string Name { get; set; }        // Название локальности
        public int DeliveryTime { get; set; }   // Срок доставки (в днях)
        public string ExportEmail { get; set; } // Почта для выгрузки файлов
    }
}
