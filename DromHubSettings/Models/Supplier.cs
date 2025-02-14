using System;

namespace DromHubSettings.Models
{
    public class Supplier
    {
        public Guid Id { get; set; }       // Уникальный идентификатор, генерируется в программе
        public string Name { get; set; }     // Название поставщика
        public string Email { get; set; }    // Почта
        public string LocalityName { get; set; } // Локальность (например, "Local", "Urban", "Regional")
        public int Index { get; set; }       // Индекс (порядковый номер)
    }
}
