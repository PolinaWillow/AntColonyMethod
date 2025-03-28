using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel.Statistic
{
    public class IntervalRecord
    {
        public string interval { get; set; }
        public TimeSpan current_all_time {get; set;}
        public TimeSpan current_findWayTask_time { get; set; }
        public TimeSpan current_senderTask_time { get; set; }

        public IntervalRecord(int interval)
        {
            this.interval = Convert.ToString(interval);
            this.current_all_time = new TimeSpan();
            this.current_findWayTask_time = new TimeSpan();
            this.current_senderTask_time = new TimeSpan();
        }

        public void Add(string typeTime, TimeSpan curentTime)
        {
            switch (typeTime)
            {
                case "all":
                    this.current_all_time = curentTime;
                    break;
                case "findWayTask":
                    this.current_findWayTask_time = curentTime;
                    break;
                case "senderTask":
                    this.current_senderTask_time = curentTime;
                    break;
            }
        }
    }
}
