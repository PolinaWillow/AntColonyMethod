using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntColonyExtLib.DataModel.Numerators;

namespace AntColonyExtLib.DataModel
{
    public class ParamDefinition
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

        public void Print() {
            Console.WriteLine("Number - " + this.numParam);
            Console.WriteLine("Type of param - "+ this.typeParam);
            Console.WriteLine("Count of values - " + this.valuesCount);
        }
    }
}
