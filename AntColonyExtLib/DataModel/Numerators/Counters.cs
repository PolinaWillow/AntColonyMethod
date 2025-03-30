using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel.Numerators
{
    public class Counters
    {
        object _monitor { get; set; } // Монитор
        public int countFindWay { get; set; } // Счетчик путей поиска
        public int countSendWay { get; set; } // Счетчик путей отправления
        public int countAgent { get; set; } // Счетчик агентов

        public Counters()
        {
            this.countFindWay = 0;
            this.countSendWay = 0;
            this.countAgent = 0;

            this._monitor = new object();
        }

        public int Add_FindWay()
        {
            Monitor.Enter(_monitor);
            this.countFindWay += 1;
            int count = this.countFindWay;
            Monitor.Exit(_monitor);

            return count;
        }

        public int Add_SendWay()
        {
            //Monitor.Enter(_monitor);
            this.countSendWay += 1;
            int count = this.countSendWay;
            //Monitor.Exit(_monitor);

            return count;
        }

        public int Add_Agents()
        {
            //Monitor.Enter(_monitor);
            this.countAgent += 1;
            int count = this.countAgent;
            //Monitor.Exit(_monitor);

            return count;
        }

    }
}
