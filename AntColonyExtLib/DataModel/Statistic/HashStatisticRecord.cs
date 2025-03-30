using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel.Statistic
{
    public class HashStatisticRecord
    {
        public int count { get; set; }
        public int from { get; set; }
        public int to { get; set; }

        public HashStatisticRecord(int from, int step)
        {
            this.count = 0;
            this.from = from+1;
            this.to = from+step;
        }


    }
}
