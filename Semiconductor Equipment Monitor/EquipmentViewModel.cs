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

namespace Semiconductor_Equipment_Monitor
{
    public class EquipmentViewModel:INotifyPropertyChanged
    {
       //MySQL 链接字符串
        private const string _connectionString =
            "server=localhost;"+
            "user=root;"+
            "password=123456;"+
            "database=WpfEquipment;"+
            "port=3306;"+
            "charset=utf8mb4";


        public ObservableCollection<Equipment> EquipmentList { get; set; }//设备列表
        public ICommand RefreshCmd { get; }//刷新按钮的MVVM命令绑定
        public ICommand QueryWorkOrderCmd { get; }//工单查询
        public ICommand AlarmRecordCmd { get; }//报警记录
        public ICommand Save2MySqlCmd { get; }//保存数据至数据库



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
            Save2MySqlCmd = new RelayCommand(SaveEquipment2MySql);//保存数据至数据库

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


        private void SaveEquipment2MySql() 
        {
            try
            {
                //1.创建数据库链接
                using (var conn = new MySqlConnection(_connectionString))
                {
                    //2.打开链接
                    conn.Open();
                    //3.清空旧数据（测试用，正式环境需删除）   命令 清空equipment表内数据
                    using (var truncateCmd = new MySqlCommand("TRUNCATE TABLE Equipment",conn)) 
                    {
                        //执行 sqlCommand 对象中定义的sql语句
                        truncateCmd.ExecuteNonQuery();
                    }

                    //4.循环插入各设备数据
                    foreach (var device in EquipmentList) 
                    {
                        string insertSql = @"
                            INSERT INTO Equipment (DeviceName, Status)
                            VALUES (@DeviceName, @Status);";//@DeviceName,@Status占位符

                        using (var cmd = new MySqlCommand(insertSql,conn)) 
                        {
                            //给参数赋值，防止SQL注入风险
                            cmd.Parameters.AddWithValue("@DeviceName",device.EqpName);
                            cmd.Parameters.AddWithValue("@Status",device.Status.ToString());

                            //执行 插入 语句
                            cmd.ExecuteNonQuery();
                        }

                    }
                }
                MessageBox.Show("✔ 设备数据已成功保存至MySQL！","成功",MessageBoxButton.OK,MessageBoxImage.Information);

            }
            catch (Exception ex)
            {

                MessageBox.Show($"❌ 保存失败：{ex.Message}","错误",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
       

    }

    


}
