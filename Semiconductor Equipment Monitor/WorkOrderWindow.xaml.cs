using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Semiconductor_Equipment_Monitor
{
    /// <summary>
    /// WorkOrderWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WorkOrderWindow : Window
    {
        public WorkOrderWindow()
        {
            DataContext = new WorkOrderViewModel();
            InitializeComponent();
        }
    }
}
