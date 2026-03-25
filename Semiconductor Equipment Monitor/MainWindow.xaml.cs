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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Semiconductor_Equipment_Monitor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new EquipmentViewModel();
        }


        ////普通事件绑定 
        //private void RefreshButtonClicked(object sender, RoutedEventArgs e)
        //{
        //    if (DataContext is EquipmentViewModel vm) { 
        //        vm.RefreshEquipmentData();
        //    }
        //}


        //刷新前点击下拉框 再点刷新 之后会触发 ComboBox_Status_SelectionChanged事件，所以加个判断
       

        //status 下拉框  改变时触发
        private void ComboBox_Status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {   //当isRefreshing=true时表示正在刷新，待刷新完成后才会进行下方事件
            if (DataContext is EquipmentViewModel vm && vm.isRefreshing)
                return;

             //获取当前选中设备                                 
            //cb.ItemsSource!=null 程序刚运行时下拉框为空，自动赋值过程中会触发Update Single.. 所以加一个不为空的判定//不过没啥用
            if (sender is ComboBox cb && cb.DataContext is Equipment eqp && e.RemovedItems.Count>0) 
            {
                EquipmentDataService.UpdateSingleEqpStatus(eqp.EqpId, eqp.Status);
            }

        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox cb) 
            {
                cb.ItemsSource = Enum.GetValues(typeof(EquipmentStatus)); 
            }
        }
    }

    
}
