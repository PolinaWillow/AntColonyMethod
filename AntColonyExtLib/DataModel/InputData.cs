using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

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

        public InputData()
        {
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
    }
}
