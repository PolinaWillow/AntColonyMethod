using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel.Statistic
{
    public class IntervalRecords_Dictionary
    {
        private object _monitor;
        public Dictionary<int, IntervalRecord> dictionary { get; set; }

        public IntervalRecords_Dictionary()
        {
            this._monitor = new object();
            this.dictionary = new Dictionary<int, IntervalRecord>();
        }

        public void Add(int interval, string typeTime, TimeSpan curentTime)
        {
            Monitor.Enter(this._monitor);
            if (!this.dictionary.ContainsKey(interval)) //Если в словаре еще не записан интервал
            {
                IntervalRecord newRecord = new IntervalRecord(interval); //Создаем новую запись для интервала
                newRecord.Add(typeTime, curentTime);

                this.dictionary.Add(interval, newRecord);

                Monitor.Exit(this._monitor);
            }
            else
            {
                IntervalRecord record = this.dictionary[interval];
                record.Add(typeTime, curentTime);
                this.dictionary[interval] = record;
                Monitor.Exit(this._monitor);
            }
        }
    }
}
