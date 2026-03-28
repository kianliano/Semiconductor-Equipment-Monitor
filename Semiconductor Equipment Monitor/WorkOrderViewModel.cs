using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Semiconductor_Equipment_Monitor
{
    public class WorkOrderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));   
        }

        private ObservableCollection<WorkOrder> _workOrderList;

        public ObservableCollection<WorkOrder> WorkOrderList
        {
            get { return _workOrderList; }
            set { 
                _workOrderList = value;
            }
        }

        public ICommand  WOSearchCmd { get;}
        public ICommand WOClearCmd { get;}



        
        public WorkOrderViewModel()
        {
            WorkOrderList = new ObservableCollection<WorkOrder>();
            LoadMockData();

            FilterWorkOrderView = CollectionViewSource.GetDefaultView(WorkOrderList);
            FilterWorkOrderView.Filter = WorkOrderFilter;

            WOSearchCmd = new RelayCommand(() => 
            {
                FilterWorkOrderView.Refresh();
            });

            WOClearCmd = new RelayCommand(() => 
            {
                WOFilterText = string.Empty;
                FilterWorkOrderView.Refresh();
            });
        }



        public void LoadMockData() 
        {
            for (int i = 1; i <= 20; i++)
            {
                WorkOrderList.Add(new WorkOrder()
                {
                    OrderId = $"Wo20260327{i:D2}",
                    Product = "芯片A",
                    Count = 5000 + 100*i,
                    Status = "正常",
                    Time = DateTime.Now
                });

            }
        }

        private string _filterText;

        public string WOFilterText
        {
            get { return _filterText; }
            set { _filterText = value;
                OnPropertyChanged(); 
                //FilterWorkOrderView.Refresh();
            }
        }

        public ICollectionView FilterWorkOrderView { get; set; }

        public bool WorkOrderFilter(object obj)
        {
            try
            {
                if (obj == null || string.IsNullOrWhiteSpace(WOFilterText))
                {
                    return true;//显示在列表中
                }
                else if (obj is WorkOrder workOrder && !string.IsNullOrEmpty(WOFilterText)) 
                {
                    bool matchOrderId = workOrder.OrderId != null && workOrder.OrderId.Contains(WOFilterText);
                    bool matchProduct = workOrder.Product != null && workOrder.Product.Contains(WOFilterText);
                    bool matchTime = workOrder.Time != null && workOrder.Time.ToString().Contains(WOFilterText);
                    return matchOrderId || matchProduct || matchTime;   
                }
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show($"搜索出错啦！{e.Message}");
            }
            return false;//不显示
        }
    }
}
