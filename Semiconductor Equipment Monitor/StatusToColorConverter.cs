using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Semiconductor_Equipment_Monitor
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)//正向转换 status->color
        {
            if (value is EquipmentStatus status) 
            {
                switch (status) { 
                    case EquipmentStatus.Running:
                        return Brushes.Green;
                    case EquipmentStatus.Alarm:
                        return Brushes.Red;
                    case EquipmentStatus.Offline:
                        return Brushes.Gray;
                    case EquipmentStatus.Idle:
                        return Brushes.Black;
                    default:
                        return Brushes.Black;
                }
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
