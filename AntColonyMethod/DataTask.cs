using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyMethod
{
    class DataTask
    {
        /// <summary>
        /// Количество параметров
        /// </summary>
        public int paramCount;

        /// <summary>
        /// Контрольное число путей
        /// </summary>
        public int controlCount { get; set; }

        /// <summary>
        /// Список для хранения количества значений параметров
        /// </summary>
        public List<int> valueCount { get; set; }

        /// <summary>
        /// Список входных значений
        /// </summary>
        public List<string> valueData { get; set; }

        /// <summary>
        ///  Хэш-таблица для хранения хэшей путей
        /// </summary>
        public Hashtable hashTable { get; set; }

        /// <summary>
        /// Граф параметров
        /// </summary>
        public Graf graf { get; set; }

        /// <summary>
        /// Граф параметров (Упорядоченная копия)
        /// </summary>
        public Graf grafCopy { get; set; }

        /// <summary>
        /// Число итераций в алгоритме
        /// </summary>
        public long iterationCount { get; set; }

        /// <summary>
        /// Количество муравьев
        /// </summary>
        public int antCount { get; set; }

        /// <summary>
        /// Наличие или отсутствие потоков
        /// </summary>
        public bool availabilityThread { get; set; }

        public DataTask() {
            valueCount = new List<int>();
            valueData = new List<string>();
            hashTable = new Hashtable();
            graf = new Graf();
            grafCopy = new Graf();
            availabilityThread = false;
            antCount = ChangeableParams.ANT_COUNT;
            iterationCount = 0;
            controlCount = 1;
        }

        /// <summary>
        /// Обнуление изменений входных данных
        /// </summary>
        public void ResetDatatTask() {
            graf.InitialGraf();
            hashTable.Clear();
        }


    }
}
