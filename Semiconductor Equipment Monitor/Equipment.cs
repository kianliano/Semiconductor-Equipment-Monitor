using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Semiconductor_Equipment_Monitor
{
    public class Equipment:INotifyPropertyChanged
    {
        public string EqpId { get; set; }//设备编号
        public string EqpName { get; set; }//设备名称
        //public EquipmentStatus Status { get; set; }//设备状态

        private EquipmentStatus _Status;//属性的完整声明

       public EquipmentStatus Status
        {
            get { return _Status; }
            set { _Status = value;
                OnPropertyChanged();//触发Property Changed事件，告知外部status发生变化
            }
        }

        public int OutputToday { get; set; }//产量
        public DateTime LastUpdateTime { get; set; } = DateTime.Now;//最后更新时间

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));//属性变化时 触发事件
        }
    }

    public enum EquipmentStatus { 
        Running,
        Alarm,
        Offline,
        Idle
    }
}
