using AntColonyExtLib.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.ClusterInteraction.Models.Calculation
{
    public class Calculation_v2
    {
        /// <summary>
        /// Путь агента для отправления
        /// </summary>
        public WayForSend[] Way_For_Send { get; set; } 

        /// <summary>
        /// Id агента
        /// </summary>
        public string idAgent { get; set; } //Id агента

        /// <summary>
        /// результат расчета
        /// </summary>
        public double result { get; set; } //результат расчета

        public void Print()
        {
            Console.Write("WayForSend: ");
            foreach (var item in Way_For_Send)
            {
                item.Print();
                Console.Write("; ");
            }
            Console.WriteLine();
            Console.WriteLine("idAgent: " + idAgent);
            Console.WriteLine("result: "+ result);
        }

        public Calculation_v2(string idAgent, int[] way, InputData inputData)
        {
            this.result = 0;
            this.idAgent = idAgent; //Добавление id агента

            //Добавление пути
            int length = way.Length;
            this.Way_For_Send = new WayForSend[length];
            for (int i = 0; i < length; i++)
            {
                this.Way_For_Send[i] = new WayForSend();
                this.Way_For_Send[i].ValueType = inputData.inputParams.Params[i].defParam.typeParam.ToString();
                //Получение параметра и значения пути
                int velCount = inputData.inputParams.Params[i].valuesParam.Count();
                for (int j = 0; j < velCount; j++)
                {
                    int id = inputData.inputParams.Params[i].valuesParam[j].idValue;
                    if (id == way[i])
                    {
                        this.Way_For_Send[i].SendValue = inputData.inputParams.Params[i].valuesParam[j].valueParam;
                        break;
                    }
                }
            }
        }

        public Calculation_v2() { }

        public string TypeOf()
        {
            return "Calculation_v2";
        }

    }
}
