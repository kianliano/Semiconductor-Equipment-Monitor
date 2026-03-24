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

namespace Semiconductor_Equipment_Monitor
{
    public class EquipmentViewModel:INotifyPropertyChanged
    {
        public ObservableCollection<Equipment> EquipmentList { get; set; }//设备列表
        public ICommand RefreshCmd { get; }//刷新按钮的MVVM命令绑定
        public ICommand QueryWorkOrderCmd { get; }//工单查询
        public ICommand AlarmRecordCmd { get; }//报警记录


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
            QueryWorkOrderCmd = new RelayCommand(QueryWorkOrder);//工单查询
            AlarmRecordCmd = new RelayCommand(ShowAlarmRecord);//报警记录
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
            foreach (var equipment in EquipmentList)
            {
                var TempData = GenerateRandomData();
                equipment.Status = TempData.Status;
                equipment.OutputToday = TempData.Value;
            }
            RefreshAllCountProperties();//主动刷新一下统计数据 保证顶部数据同步
        }

        public void QueryWorkOrder()//工单查询
        {
            MessageBox.Show("正在查询工单.....");
        }


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


       

    }

    


}
