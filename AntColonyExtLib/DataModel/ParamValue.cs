using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel
{
    public class ParamValue
    {
        /// <summary>
        /// Id значения параметрв
        /// </summary>
        public int idValue { get; set; }

        /// <summary>
        /// Номер значения параметра
        /// </summary>
        public int numValue { get; set; }

        /// <summary>
        /// Значение параметра
        /// </summary>
        public string valueParam { get; set; }

        /// <summary>
        /// Количество феромонов
        /// </summary>
        public double pheromones { get; set; }

        public ParamValue() {
            idValue = 0;
            numValue = 0;
            valueParam= "";
            pheromones= 0;
        }

        public void Print() {
            Console.WriteLine("id - " + this.idValue + "; num - "+ this.numValue+"; value - "+this.valueParam+"; pheromones - "+this.pheromones) ;
        }

    }
}
