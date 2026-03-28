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
    public class AlarmRecordViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) 
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs (propertyName));
        }


        private ObservableCollection<AlarmRecord> _alarmList;

        public ObservableCollection<AlarmRecord> AlarmList
        {
            get { return _alarmList; }
            set { _alarmList = value;
                  OnPropertyChanged();
                }
        }
        public ICommand ARVSearchCmd { get;}//初始化后一般无需改变 所以为只读即可
        public ICommand ARVClearCmd { get;}
        public ICommand ARVSaveCmd { get;}


        public AlarmRecordViewModel()//构造函数
        {
            AlarmList = new ObservableCollection<AlarmRecord>();
            LoadAlarmRecord();

            FilterAlarmRecordView = CollectionViewSource.GetDefaultView(AlarmList);//包装一层
            FilterAlarmRecordView.Filter = AlarmRecordFilter;//filter

            ARVSearchCmd = new RelayCommand(() => 
            {
                FilterAlarmRecordView.Refresh();
            });
            ARVClearCmd = new RelayCommand(() => 
            {
                FilterText = string.Empty;
                FilterAlarmRecordView.Refresh();//恢复
            
            });
            ARVSaveCmd = new RelayCommand(() => 
            {
                System.Windows.MessageBox.Show("保存成功！");//暂时功能
            });


        }

        public void LoadAlarmRecord() 
        {
            Random random = new Random();
            for (int i = 1; i <= 20; i++)
            {
                int index = random.Next(Enum.GetValues(typeof(AlarmMessageInfo)).Length);//枚举的长度
                var alarmRecord = new AlarmRecord()
                {
                    AlarmId = $"AL{index:D2}",
                    AlarmMessage = (AlarmMessageInfo)index,
                    Time = DateTime.Now,
                    EqpId = $"EQP-{random.Next(20):D3}",
                    isHandled = random.Next(2) ==0?true:false
                };
                AlarmList.Add(alarmRecord);
            }
        }

        private string  _filterText;

        public string   FilterText
        {
            get { return _filterText; }
            set { 
                _filterText = value;
                 OnPropertyChanged();
                 //FilterAlarmRecordView.Refresh();//实施筛选 
                
            }
        }
        public ICollectionView FilterAlarmRecordView { get; set; }//包装
        public bool AlarmRecordFilter(object obj)
        {
            try
            { 
                if (string.IsNullOrWhiteSpace(FilterText))
                    return true;
                else if (!string.IsNullOrEmpty(FilterText) && obj is AlarmRecord alarmRecord)
                {
                    //先判断是否为空 再进行过滤
                    bool matchAlarmId = alarmRecord.AlarmId != null && alarmRecord.AlarmId.Contains(FilterText);
                    bool matchEqpid = alarmRecord.EqpId != null && alarmRecord.EqpId.Contains(FilterText);
                    bool matchAlarmTime = alarmRecord.Time != null && alarmRecord.Time.ToString().Contains(FilterText);

                    return matchAlarmId || matchEqpid || matchAlarmTime;
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
