using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel
{
    internal class ParamsList
    {
        /// <summary>
        /// Список параметров
        /// </summary>
        public List<Param> Params { get; set; }

        /// <summary>
        /// Количество всех возможных комбинаций значений
        /// </summary>
        public int countCombinationsV { get; set; }

        public ParamsList() {
            Params = new List<Param>();
            countCombinationsV = 0;
        }
    }
}
