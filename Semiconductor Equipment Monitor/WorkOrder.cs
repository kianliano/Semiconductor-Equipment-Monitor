using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semiconductor_Equipment_Monitor
{
    public class WorkOrder
    {
        public string OrderId { get; set; }//工单号
        public string Product { get; set; }//产品
        public int Count { get; set; }//数量
        public string Status { get; set; }//状态
        public DateTime Time { get; set; }//时间
    }
}
