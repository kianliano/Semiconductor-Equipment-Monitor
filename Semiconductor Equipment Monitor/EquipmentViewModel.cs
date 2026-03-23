using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Semiconductor_Equipment_Monitor
{
    public class EquipmentViewModel
    {
        public ObservableCollection<Equipment> EquipmentList { get; set; }//设备列表
        public EquipmentViewModel()//构造函数
        {
            EquipmentList = new ObservableCollection<Equipment>();
            for (int i = 1; i <= 20; i++)
            {
                EquipmentList.Add(new Equipment
                {
                    EqpId = $"EQP-{i:D3}",
                    EqpName = $"设备{i}",
                    Status = i%3 == 0 ? EquipmentStatus.Alarm:
                             i%5 == 0 ? EquipmentStatus.Offline:EquipmentStatus.Running
                });
            }
        }
    }
}
