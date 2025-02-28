using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace DromHubSettings.Converters
{
    public class IntToColumnLetterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int columnIndex)
            {
                return ColumnNumberToName(columnIndex);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string columnLetter)
            {
                return ColumnNameToNumber(columnLetter);
            }
            return 0;
        }

        private string ColumnNumberToName(int number)
        {
            string columnName = "";
            while (number > 0)
            {
                int remainder = (number - 1) % 26;
                columnName = (char)(remainder + 'A') + columnName;
                number = (number - remainder) / 26;
            }
            return columnName;
        }

        private int ColumnNameToNumber(string name)
        {
            int sum = 0;
            foreach (char c in name.ToUpper())
            {
                sum *= 26;
                sum += (c - 'A' + 1);
            }
            return sum;
        }
    }

}
