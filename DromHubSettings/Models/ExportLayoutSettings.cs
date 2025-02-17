using System;

namespace DromHubSettings.Models
{
    public class ExportLayoutSettings
    {
        public Guid Id { get; set; }
        public int StartRow { get; set; } // Номер строки, с которой начинается запись (например, 2)
    }
}