using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semiconductor_Equipment_Monitor
{
    internal class AlarmRecordViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<AlarmRecord> AlarmList { get; set; }
        

        public AlarmRecordViewModel()
        {
            AlarmList = new ObservableCollection<AlarmRecord>();
            LoadAlarmRecord();
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
    }
}
