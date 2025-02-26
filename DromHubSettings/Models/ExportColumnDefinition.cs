using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DromHubSettings.Models
{
    /// <summary>
    /// Описание отдельного столбца в Excel.
    /// </summary>
    public class ExportColumnDefinition
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Ссылка на родительский ExportLayout (по сути, "какому набору" принадлежит столбец).
        /// </summary>
        public Guid ExportLayoutId { get; set; }

        /// <summary>
        /// Заголовок столбца (например, "Артикул").
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Номер столбца (1, 2, 3...).
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// Активен ли столбец (показывать/не показывать в Excel).
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Какое поле из ваших данных подставлять (например, "CatalogNumber", "Brand" и т.д.).
        /// </summary>
        public string MappingProperty { get; set; }
    }
}
