using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            hashTable = new Hashtable();
            iterationCount = 0;
            antCount = 0;
        }
    }
}
