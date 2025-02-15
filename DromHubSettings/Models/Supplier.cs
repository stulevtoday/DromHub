using System;

namespace DromHubSettings.Models
{
    public class Supplier
    {
        public Guid Id { get; set; }       // Уникальный идентификатор, генерируется в программе
        public string Name { get; set; }     // Название поставщика
        public string Email { get; set; }    // Почта
        public Guid LocalityId { get; set; }  // теперь хранится id локальности
        public int Index { get; set; }       // Индекс (порядковый номер)
        public bool IsActive { get; set; } = true;

        // Это поле не хранится в таблице supplier, но получаем через JOIN
        public string LocalityName { get; set; }
    }
}
