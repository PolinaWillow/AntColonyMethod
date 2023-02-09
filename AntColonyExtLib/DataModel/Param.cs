using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel
{
    internal class Param
    {
        /// <summary>
        /// Определение параметра
        /// </summary>
        public ParamDefinition defParam { get; set; }

        /// <summary>
        /// Список значений параметра
        /// </summary>
        public List<ParamValue> valuesParam { get; set; }

        public Param() {
            defParam = new ParamDefinition();
            valuesParam = new List<ParamValue>();
        }
    }
}
