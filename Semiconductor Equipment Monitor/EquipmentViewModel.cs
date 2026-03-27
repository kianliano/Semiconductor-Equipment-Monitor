using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Windows;
using System.Data;
using MySqlConnector;
using System.Windows.Documents;
using System.Windows.Data;
using System.Threading;
using System.Security.Cryptography.X509Certificates;

namespace Semiconductor_Equipment_Monitor
{
    public class EquipmentViewModel:INotifyPropertyChanged
    {
        //下拉框 里的值 需要枚举 
        //public IEnumerable<EquipmentStatus> ts = Enum.GetValues(typeof(EquipmentStatus)).Cast<EquipmentStatus>();

        private bool _isRefreshing = false;//加点判断 防止刷新时一直触发 下拉框更改事件

        public bool isRefreshing
        {
            get { return _isRefreshing; }
            set { _isRefreshing = value;
                OnPropertyChanged();
            }
        }



        public ObservableCollection<Equipment> EquipmentList { get; set; }//设备列表
        public ICommand RefreshCmd { get; }//刷新按钮的MVVM命令绑定
        public ICommand QueryWorkOrderCmd { get; }//工单查询
        public ICommand AlarmRecordCmd { get; }//报警记录
        public ICommand Save2MySqlCmd { get; }//保存数据至数据库
        public ICommand LoadFromSqlCmd { get;}//从数据库读取数据
        public ICommand ClearFilterCmd { get; set; }//清除搜索框
        //public ICommand SearchCmd { get; set; }//搜索框输入完成按enter
        public ICommand SearchCmd { get;  }//搜索框输入完成按enter  
        //必须为只读属性 初始化之后不会为空 若是正常getset会导致赋值null 造成 未将对象引用设置到对象的实例

        ////临时测试update方法
        //public ICommand UpdateStatusCmd { get; }



        //统计运行状态
        public int TotalCount => EquipmentList.Count;//属性count
        public int RunningCount => EquipmentList.Count(e => e.Status == EquipmentStatus.Running);//linq方法count
        public int AlarmCount => EquipmentList.Count(e => e.Status == EquipmentStatus.Alarm);
        public int OfflineCount => EquipmentList.Count(e => e.Status == EquipmentStatus.Offline);
        public int IdleCount => EquipmentList.Count(e => e.Status == EquipmentStatus.Idle);
        

        public EquipmentViewModel()//构造函数
        {
            EquipmentList = new ObservableCollection<Equipment>();
            InitEquipmentData();
            EquipmentList.CollectionChanged += EquipmentList_CollectionChanged;
            
            //控件直接绑定view model 可随时更换RefreshEquipmentData方法 实现强大的解耦功能
            RefreshCmd = new RelayCommand(RefreshEquipmentData);//刷新
            QueryWorkOrderCmd = new RelayCommand(() => 
            {
                var WOW = new WorkOrderWindow();
                WOW.ShowDialog();//模态窗口 关了才能开下一个窗口
            
            });//工单查询


            AlarmRecordCmd = new RelayCommand(() => 
            {
                var ARW = new AlarmRecordWindow();
                ARW.Show();//可以开多个窗口
            });//报警记录

            

            /*Save2MySqlCmd = new RelayCommand(SaveEquipment2MySql);*///保存数据至数据库
            //Save2MySqlCmd = new RelayCommand(EquipmentDataService.SaveEquipment2MySql(EquipmentList));错误 action委托无法引用带参数方法
            //用lambda表达式（）无参方法 调用 带参数方法
            Save2MySqlCmd = new RelayCommand(() =>EquipmentDataService.SaveEquipment2MySql(EquipmentList));
            LoadFromSqlCmd = new RelayCommand(() => EquipmentDataService.LoadFromMySql(EquipmentList));
            ////测试//ok
            //UpdateStatusCmd = new RelayCommand(() => EquipmentDataService.UpdateSingleEqpStatus("EQP-001",EquipmentStatus.Running));


            //filter
            FilterEqpView = CollectionViewSource.GetDefaultView(EquipmentList);//包装一层 
            FilterEqpView.Filter = EqpFilter;//Filter为predicate委托，参数类型必须是object类型

            SearchCmd = new RelayCommand(() => FilterEqpView.Refresh());
            ClearFilterCmd = new RelayCommand( () => 
            { 
                FilterText = string.Empty;
                FilterEqpView.Refresh();
            });//清空
        }

       

        private void InitEquipmentData()//初始化设备列表
        {
            //Random random = new Random();
            //EquipmentStatus[] statuses = {EquipmentStatus.Idle,EquipmentStatus.Alarm,EquipmentStatus.Offline,EquipmentStatus.Running};
            for (int i = 1; i <= 20; i++)
            {
                var equipment = new Equipment()
                {
                    EqpId = $"EQP-{i:D3}",//i:D3 即001或者018或者206 表示三位数
                    EqpName = $"设备{i}",
                    //Status = i%3 == 0 ? EquipmentStatus.Alarm:
                    //         i%5 == 0 ? EquipmentStatus.Offline:
                    //         i%7 == 0? EquipmentStatus.Idle: EquipmentStatus.Running
                    //Status = statuses[random.Next(0,4)],//状态随机
                    //OutputToday = random.Next(500,2000),//产量随机
                };
                var TempData = GenerateRandomData();
                equipment.Status = TempData.Status;
                equipment.OutputToday = TempData.Value;
                equipment.PropertyChanged += Equipment_PropertyChanged;
                EquipmentList.Add(equipment);
                
            }
        }
        public void RefreshEquipmentData()//刷新设备状态与产量
        {
            isRefreshing = true;//上锁-开始刷新
            foreach (var equipment in EquipmentList)
            {
                var TempData = GenerateRandomData();
                equipment.Status = TempData.Status;
                equipment.OutputToday = TempData.Value;
            }
            RefreshAllCountProperties();//主动刷新一下统计数据 保证顶部数据同步
            isRefreshing = false;//刷新结束-解锁
        }

        //public void QueryWorkOrder()//工单查询
        //{
        //    MessageBox.Show("正在查询工单.....");
        //}


        public void ShowAlarmRecord()//报警记录
        {
            MessageBox.Show("正在查看报警记录.....");
        }


        

        private readonly Random _random = new Random();//_random私有字段 防止刷新太频繁 出现重复
        private (EquipmentStatus Status,int Value) GenerateRandomData() 
        {  
            EquipmentStatus[] statuses = { EquipmentStatus.Idle, EquipmentStatus.Alarm, EquipmentStatus.Offline, EquipmentStatus.Running };
            EquipmentStatus TempStatus = statuses[_random.Next(0,4)];
            int TempValue = _random.Next(500,2000);
            return ( TempStatus,TempValue);
        }
        /// <summary>
        /// /out参数👇  元组👆 一个方法一次性返回多个不同类型的值 
        /// </summary>
        //private void GenerateRandomData(out EquipmentStatus status,out int value) 
        //{
        //    EquipmentStatus[] statuses = { EquipmentStatus.Idle, EquipmentStatus.Alarm, EquipmentStatus.Offline, EquipmentStatus.Running };
        //    status = statuses[_random.Next(0, 4)];
        //    value = _random.Next(500, 2000);
        //}



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //属性变化时，触发事件
        }

        public void RefreshAllCountProperties()//刷新所有统计属性
        {
            OnPropertyChanged(nameof(TotalCount));
            OnPropertyChanged(nameof(IdleCount));   
            OnPropertyChanged(nameof(RunningCount));
            OnPropertyChanged(nameof(AlarmCount));
            OnPropertyChanged(nameof(OfflineCount));

        }
        private void EquipmentList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshAllCountProperties();

            if (e.NewItems != null) 
            {
                foreach (Equipment eq in e.NewItems) //新增设备时 为新设备订阅 状态变更事件
                {
                    eq.PropertyChanged += Equipment_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Equipment eq in e.OldItems) //删除设备时 为旧设备 取消订阅//避免内存泄露
                {
                    eq.PropertyChanged -= Equipment_PropertyChanged;
                }
            } 

        }

        private void Equipment_PropertyChanged(object sender, PropertyChangedEventArgs e)//单个设备状态变化时触发
        {
            if (e.PropertyName == nameof(Equipment.Status)) //仅当status属性变化 触发
            {
                RefreshAllCountProperties();
            }
        }



        private string _filterText ;
        //private CancellationTokenSource _filterDelayToken;//C#自带的取消令牌，专门用来叫停任务

        public string FilterText 
        {
            get { return _filterText; }
            set { 
                 _filterText = value;
                if (_filterText != "请输入搜索内容") 
                {
                    OnPropertyChanged();
                    

                    //////取消之前延时  
                    ////_filterDelayToken?.Cancel();
                    //_filterDelayToken = new CancellationTokenSource();
                    ////停下300毫秒再搜索
                    //Task.Delay(3000, _filterDelayToken.Token).ContinueWith(t =>
                    //{
                    //    if (!t.IsCanceled)
                    //    { Application.Current.Dispatcher.Invoke(() => FilterEqpView.Refresh()); }
                    //});
                }
            }
        }

        public ICollectionView FilterEqpView { get; set; }//icollectionview 对数据进行筛选，排序，分组

        public bool EqpFilter(object obj)
        {
            try
            {
                if (FilterText == "请输入搜索内容")
                    return true;
                else if (string.IsNullOrWhiteSpace(FilterText))
                    return true;
                if (!string.IsNullOrEmpty(FilterText) && obj is Equipment equipment)
                {
                    //return equipment.EqpName?.Contains(FilterText)??false
                    //    || equipment.DeviceName.Contains(FilterText)
                    //    || equipment.EqpId.Contains(FilterText)
                    //    || equipment.LineId.Contains(FilterText);

                    //先判断是否为空 再进行过滤
                    bool matchEqpName = equipment.EqpName != null && equipment.EqpName.Contains(FilterText);
                    bool matchDeviceName = equipment.DeviceName !=null && equipment.DeviceName.Contains(FilterText);
                    bool matchEqpid = equipment.EqpId != null && equipment.EqpId.Contains(FilterText);
                    bool matchLineid = equipment.LineId !=null && equipment.LineId.Contains(FilterText);

                    return matchEqpName || matchDeviceName || matchEqpid || matchLineid;
                }

                return false;
            }
            catch (Exception ex)
            {

                MessageBox.Show($"搜索出错啦：{ex.Message}");
                return false;
            }
            
        }



    }

    


}
