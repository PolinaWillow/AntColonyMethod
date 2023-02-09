using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel
{
    internal class ParamDefinition
    {
        /// <summary>
        /// Номер параметра
        /// </summary>
        public int numParam { get; set; }

        /// <summary>
        /// Тип параметра
        /// </summary>
        public TypeNumerator typeParam { get; set; }

        /// <summary>
        /// Количество значений параметра
        /// </summary>
        public int valuesCount { get; set; }

        public ParamDefinition() {
            numParam= 0;
            typeParam= TypeNumerator.None; 
            valuesCount= 0;
        }
    }
}
