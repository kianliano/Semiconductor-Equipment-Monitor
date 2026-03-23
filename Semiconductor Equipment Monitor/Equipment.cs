using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semiconductor_Equipment_Monitor
{
    public class Equipment
    {
        public string EqpId { get; set; }//设备编号
        public string EqpName { get; set; }//设备名称
        public EquipmentStatus Status { get; set; }//设备状态
    }

    public enum EquipmentStatus { 
        Running,
        Alarm,
        Offline
    }
}
