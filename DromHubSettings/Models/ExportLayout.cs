using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DromHubSettings.Models
{
    /// <summary>
    /// Общие настройки разметки для экспорта Excel.
    /// Содержит начальную строку и коллекцию столбцов.
    /// </summary>
    public class ExportLayout
    {
        public Guid Id { get; set; }

        /// <summary>
        /// С какой строки начинать вывод в Excel.
        /// </summary>
        public int StartRow { get; set; }

        /// <summary>
        /// Список столбцов (Артикул, Бренд, Цена и т.д.).
        /// </summary>
        public List<ExportColumnDefinition> Columns { get; set; } = new List<ExportColumnDefinition>();
    }
}
