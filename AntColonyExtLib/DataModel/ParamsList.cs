using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AntColonyExtLib.DataModel
{
    public class ParamsList
    {
        /// <summary>
        /// Список параметров
        /// </summary>
        public List<Param> Params { get; set; }

        /// <summary>
        /// Количество всех возможных комбинаций значений
        /// </summary>
        public int countCombinationsV { get; set; }

        public ParamsList()
        {
            Params = new List<Param>();
            countCombinationsV = 0;
        }

        public object Clone()
        {
            ParamsList objClone = new ParamsList();
            objClone.countCombinationsV = this.countCombinationsV;
            objClone.Params = new List<Param>();
            foreach (Param param in this.Params)
            {
                objClone.Params.Add((Param)param.Clone());
            }
            return objClone;
        }

        public void Print()
        {
            Console.WriteLine("Count of values conbinations - " + this.countCombinationsV);
            foreach (var elem in this.Params)
            {
                elem.Print();
            }
        }

        //Поиск пути в графе
        public int[] FindWay()
        {
            int[] way = new int[this.Params.Count()]; //Выбранный путь           


            for (int i = 0; i < this.Params.Count(); i++)
            {
                //Выбор значения параметра
                ChoiceNextVertex_v2(this.Params[i].defParam.numParam, this.Params[i].valuesParam, way);
                //ChoiceNextVertex(this.Params[i].defParam.numParam, this.Params[i].valuesParam, way);
            }

            return way;
        }

        /// <summary>
        /// Выбор следцющей вершины
        /// </summary>
        /// <param name="NumParam">Номем парамертра </param>
        /// <param name="valuesParam">Список значений</param>
        /// <param name="way">Путь</param>
        /// <returns></returns>
        private int ChoiceNextVertex(int NumParam, List<ParamValue> valuesParam, int[] way) //Выбор следующей вершины
        {

            double sumPheromones = 0;
            double[] Pij = new double[valuesParam.Count()];//Массив вероятности попадания

            //Определяем суммарное количество феромонов
            for (int i = 0; i < valuesParam.Count(); i++)
            {
                sumPheromones += valuesParam[i].pheromones;
            }
            //Подсчет вероятности выбора кждого узла
            for (int i = 0; i < valuesParam.Count(); i++)
            {
                Pij[i] = valuesParam[i].pheromones / sumPheromones;
            }
            //Выбор значения параметра
            double[] intervals = new double[valuesParam.Count() + 1]; //Определение интервалов попадания
            intervals[0] = 0;
            for (int i = 1; i < intervals.Length; i++)
            {
                intervals[i] = intervals[i - 1] + Pij[i - 1];
            }
            intervals[intervals.Length - 1] = 1;

            Random rnd = new Random();
            double value = rnd.NextDouble();

            int numValue = 0;
            int x = 1;
            while (numValue == 0)
            {
                if ((value < intervals[x]) && (value > intervals[x - 1]))
                {
                    numValue = x;
                }
                x++;
            }
            way[NumParam] = valuesParam[numValue - 1].idValue;
            return 0;
        }


        /// <summary>
        /// Выбор следцющей вершины
        /// </summary>
        /// <param name="NumParam">Номем парамертра </param>
        /// <param name="valuesParam">Список значений</param>
        /// <param name="way">Путь</param>
        /// <returns></returns>
        public int ChoiceNextVertex_v2(int NumParam, List<ParamValue> valuesParam, int[] way)
        {
            //Коэффициенты при слагаемых
            double k1 = 1; double k2 = 1; double k3 = 1;
            double sumPheromones = 0; //Общее количество феромонов в слое


            double[] Pij = new double[valuesParam.Count()];//Массив вероятности попадания

            //Определяем суммарное количество феромонов
            for (int i = 0; i < valuesParam.Count(); i++)
            {
                sumPheromones += valuesParam[i].pheromones;
            }

            double denominator = 0;
            for (int i = 0; i < valuesParam.Count(); i++)
            {
                double saturation = valuesParam[i].saturation;
                if (saturation == 0) saturation = 1;
                denominator += k1 * (valuesParam[i].pheromones / sumPheromones) + k2 * (1 / saturation) + k3 * (saturation / this.countCombinationsV);
            }

            //Подсчет вероятности выбора кждого узла
            for (int i = 0; i < valuesParam.Count(); i++)
            {
                double saturation = valuesParam[i].saturation;
                if (saturation == 0) saturation = 1;

                double nominator = k1 * (valuesParam[i].pheromones / sumPheromones) + k2 * (1 / saturation) + k3 * (saturation / this.countCombinationsV);
                Pij[i] = nominator / denominator;
            }
            //Выбор значения параметра
            double[] intervals = new double[valuesParam.Count() + 1]; //Определение интервалов попадания
            intervals[0] = 0;
            for (int i = 1; i < intervals.Length; i++)
            {
                intervals[i] = intervals[i - 1] + Pij[i - 1];
            }
            intervals[intervals.Length - 1] = 1;

            Random rnd = new Random();
            double value = rnd.NextDouble();

            int numValue = 0;
            int x = 1;
            while (numValue == 0)
            {
                if ((value < intervals[x]) && (value > intervals[x - 1]))
                {
                    numValue = x;
                }
                x++;
            }
            way[NumParam] = valuesParam[numValue - 1].idValue;

            return 0;
        }


    }
}
