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

        /// <summary>
        /// Насыщение вершины (сколько раз использовалась в путях агентов)
        /// </summary>
        public int saturation { get; set; }

        public ParamValue()
        {
            idValue = 0;
            numValue = 0;
            valueParam = "";
            pheromones = 0;
            saturation = 0;
        }

        public object Clone()
        {
            ParamValue objClone = new ParamValue();
            objClone.idValue = this.idValue;
            objClone.numValue = this.numValue;
            objClone.valueParam = this.valueParam;
            objClone.pheromones = this.pheromones;
            objClone.saturation = this.saturation;

            return objClone;
        }

        public int CheckId(int id)
        {
            if (idValue != id) { return -1; }
            return 0;
        }

        public void Print()
        {
            Console.WriteLine("id - " + this.idValue + "; num - " + this.numValue + "; value - " + this.valueParam + "; pheromones - " + this.pheromones + "; saturation - " + this.saturation);
        }

    }
}
