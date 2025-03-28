using AntColonyExtLib.DataModel.Statistic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace AntColonyExtLib.DataModel
{
    public class InputData
    {
        /// <summary>
        /// Входные параметры
        /// </summary>
        public ParamsList inputParams { get; set; }

        /// <summary>
        /// Копия исходного набора параметров
        /// </summary>
        public ParamsList cloneInputParams { get; set; }

        /// <summary>
        /// Флаг внесения изменений в граф (true - отправляем на cloneInputParams false - на inputParams)
        /// </summary>
        public bool changeFlag { get; set; }

        /// <summary>
        /// Таблица хэшей
        /// </summary>
        public Hashtable hashTable { get; set; }

        /// <summary>
        /// Количество итераций
        /// </summary>
        public long iterationCount { get; set; }

        /// <summary>
        /// Количество агентов
        /// </summary>
        public int antCount { get; set; }

        /// <summary>
        /// Монитор
        /// </summary>
        private object _monitor;


        /// <summary>
        /// Время поиска хэша в таблице
        /// </summary>
        private TimeStatistic _timer { get; set; }


        public InputData()
        {
            _monitor = new object();

            _timer = new TimeStatistic("TimeStatistic_hashTable");
            this._timer.Write("Время поиска в hash-таблице");

            inputParams = new ParamsList();
            cloneInputParams = new ParamsList();

            changeFlag = false;

            hashTable = new Hashtable();
            iterationCount = 0;
            antCount = 0;
        }

        public void Print() {
            Console.WriteLine("Ant count - "+this.antCount);
            Console.WriteLine("Iteraton count - " + this.iterationCount);
            Console.WriteLine("Params:");
            inputParams.Print();
        }

        public int UpdateParams()
        {
            this.inputParams = (ParamsList)this.cloneInputParams.Clone();
            this.changeFlag = false;
            return 0;
        }

        /// <summary>
        /// Добавление хэша в хэш таблицу
        /// </summary>
        /// <param name="hashWay">hash</param>
        /// <param name="wayAgent">путь агента в графе</param>
        /// <returns></returns>
        public int AddNewHash(string hashWay, int[] wayAgent)
        {
            Monitor.Enter(_monitor);
            _timer.TimeStatistic_Start("findHashTable");
            bool isInHashTable = this.hashTable.ContainsKey(hashWay);
            _timer.TimeStatistic_End("findHashTable");
            //_timer.Write(null, "findHashTable");
            if (!isInHashTable)
            {
                this.hashTable.Add(hashWay, wayAgent);
                Monitor.Exit(_monitor);
                return 0;

            }
            else
            {
                Monitor.Exit(_monitor);
                return -1;
            }
        }
    }
}
