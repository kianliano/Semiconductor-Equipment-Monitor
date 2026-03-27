using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semiconductor_Equipment_Monitor
{
    public class AlarmRecord
    {
        public AlarmMessageInfo AlarmMessage { get; set; }//报警信息
        public string AlarmId { get; set; }//报警编号
        public string EqpId { get; set; }//设备名
        public DateTime Time { get; set; }//报警时间
        public bool isHandled { get; set; }//是否处理
    }

    public enum AlarmMessageInfo 
    {
      设备温度过高,
      设备压力异常,
      设备任务已完成,
      设备存在异常掉电
    }

}
