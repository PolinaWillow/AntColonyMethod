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
        public int controlCount;

        /// <summary>
        /// Список для хранения количества значений параметров
        /// </summary>
        public List<int> valueCount = new List<int>();

        /// <summary>
        /// Список входных значений
        /// </summary>
        public List<string> valueData = new List<string>();

        /// <summary>
        ///  Хэш-таблица для хранения хэшей путей
        /// </summary>
        public Hashtable hashTable = new Hashtable();

        public Graf graf = new Graf();

        /// <summary>
        /// Наличие или отсутствие потоков
        /// </summary>
        public bool availabilityThread = false;
    }
}
