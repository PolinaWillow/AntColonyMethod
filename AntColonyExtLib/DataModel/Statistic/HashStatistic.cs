using AntColonyExtLib.Processing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel.Statistic
{
    public class HashStatistic
    {
        List<HashStatisticRecord> dictionary { get; set; } //Dictionary<int, int> dictionary { get; set; }
        int _iterationStep { get; set; }
        int _MaxIteration { get; set; }

        public HashStatistic(int iterationStep, int MaxIteration)
        {
            this.dictionary = new List<HashStatisticRecord>(); //new Dictionary<int, int>();
            this._iterationStep = iterationStep;
            this._MaxIteration = MaxIteration;

            int count = 0;
            
            while (count <= MaxIteration)
            {
                HashStatisticRecord record = new HashStatisticRecord(count, iterationStep);
                dictionary.Add(record);
                count += iterationStep;
            }

            if (count - MaxIteration > 0 && count-MaxIteration!=iterationStep)
            {
                HashStatisticRecord record = new HashStatisticRecord(count- iterationStep, MaxIteration-(count- iterationStep));
            }

            //int count = iterationStep;
            //while (count< MaxIteration)
            //{
            //    dictionary.Add(count, 0);
            //    count += iterationStep;
            //}

            //dictionary.Add(MaxIteration, 0);
        }
       
        public void Count(int iteration)
        {
            foreach (var record in dictionary)
            {
                if (iteration>=record.from && iteration <= record.to)
                {
                    record.count++;
                    return;
                }
            }
            //foreach (var counter in dictionary)
            //{
            //    if (counter.Key-this._iterationStep <= iteration && counter.Key > iteration)
            //    {
            //        dictionary[counter.Key] = counter.Value + 1;
            //        return;
            //    }
            //}
        }

        public void Clear()
        {
            this.dictionary = new List<HashStatisticRecord>(); //new Dictionary<int, int>();

            int count = 0;

            while (count <= this._MaxIteration)
            {
                HashStatisticRecord record = new HashStatisticRecord(count, this._iterationStep);
                dictionary.Add(record);
                count += this._iterationStep;
            }

            if (count - this._MaxIteration > 0 && count - this._MaxIteration != this._iterationStep)
            {
                HashStatisticRecord record = new HashStatisticRecord(count - this._iterationStep, this._MaxIteration - (count - this._iterationStep));
            }
            //this.dictionary = new Dictionary<int, int>();
            //int count = this._iterationStep;
            //while (count < this._MaxIteration)
            //{
            //    dictionary.Add(count, 0);
            //    count += this._iterationStep;
            //}

            //dictionary.Add(this._MaxIteration, 0);
        }

        public void Print()
        {
            Console.WriteLine("HashStatistic");
            foreach (var counter in dictionary)
            {
                string printStr = "[ " + counter.from + "; " + counter.to + "] \t " + counter.count;
                //string printStr = counter.Key + ":\t" + counter.Value;
                Console.WriteLine(printStr);
            }
        }
    }
}
