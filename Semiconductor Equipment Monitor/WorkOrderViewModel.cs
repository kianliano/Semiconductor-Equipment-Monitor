using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semiconductor_Equipment_Monitor
{
    public class WorkOrderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<WorkOrder> WorkOrders { get; set; }
        public WorkOrderViewModel()
        {
            WorkOrders = new ObservableCollection<WorkOrder>();
            LoadMockData();
            
        }

        public void LoadMockData() 
        {
            for (int i = 1; i <= 20; i++)
            {
                WorkOrders.Add(new WorkOrder()
                {
                    OrderId = $"Wo20260327{i:D2}",
                    Product = "芯片A",
                    Count = 5000 + 100*i,
                    Status = "正常",
                    Time = DateTime.Now
                });

            }
        }
    }
}
