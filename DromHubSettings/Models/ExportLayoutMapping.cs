using System;

namespace DromHubSettings.Models
{
    public class ExportLayoutMapping
    {
        public Guid Id { get; set; }           // Уникальный идентификатор маппинга
        public string FieldKey { get; set; }     // Идентификатор поля (например, "catalog", "brand", "name", "price", "quantity", "delivery", "multiple", "image")
        public string HeaderText { get; set; }   // Заголовок, который будет записываться в Excel (например, "Артикул", "Бренд" и т.д.)
        public int ColumnNumber { get; set; }    // Номер столбца для данного поля
    }
}