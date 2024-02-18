using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel
{
    public class Param
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

        public object Clone()
        {
            Param objClone = new Param();
            objClone.defParam = (ParamDefinition)this.defParam.Clone();
            foreach (ParamValue v in this.valuesParam)
            {
                objClone.valuesParam.Add((ParamValue)v.Clone());
            }
            return objClone;
        }

        public int UpdateSaturation(int valueId, int deltaSaturation)
        {
            foreach (ParamValue v in valuesParam) {
                if (v.CheckId(valueId) == 0)
                {
                    v.saturation = v.saturation + deltaSaturation;
                    return 0;
                }
            }

            return -1;
        }

        public void Print() {
            Console.WriteLine("\n");
            this.defParam.Print();
            foreach (var elem in this.valuesParam) {
                elem.Print();
            }
        }

        /// <summary>
        /// Инициализация параметра
        /// </summary>
        /// <returns></returns>
        public int InitialStateParam()
        {
            foreach (ParamValue v in valuesParam) {
                v.pheromones = 1;
            }
            return 0;
        }
    }
}
