using AntColonyExtLib.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AntColonyExtLib.ClusterInteraction.Models.Calculation
{
    public class MultyCalculation
    {
        /// <summary>
        /// Список отправляемых данных
        /// </summary>
        public List<Calculation_v2> calculationList { get; set; } 
        
        public MultyCalculation() {
            this.calculationList = new List<Calculation_v2>();
        }

        public MultyCalculation(List<Calculation_v2> calculationList)
        {
            this.calculationList = new List<Calculation_v2>();
            foreach(Calculation_v2 item in calculationList)
            {
                this.calculationList.Add(item);
            }
        }


        /// <summary>
        /// Добавление данных для отправления
        /// </summary>
        /// <param name="idAgent">id агента</param>
        /// <param name="way">Массив id значений параметров</param>
        /// <param name="inputData">Граф</param>
        public void Add(string idAgent, int[] way, InputData inputData)
        {
            Calculation_v2 calculation = new Calculation_v2( idAgent, way, inputData);
            this.calculationList.Add(calculation);
        }

        /// <summary>
        /// Добавление данных для отправления
        /// </summary>
        /// <param name="idAgent">id агента</param>
        /// <param name="way">Массив id значений параметров</param>
        /// <param name="inputData">Граф</param>
        public void Add(Calculation_v2 calculation)
        {
            //Calculation_v2 calculation = new Calculation_v2(idAgent, way, inputData);
            this.calculationList.Add(calculation);
        }

        public void Add(List<Calculation_v2> calculationList)
        {
            this.calculationList.AddRange(calculationList);
        }

        /// <summary>
        /// Получение типа отправляемых данных
        /// </summary>
        /// <returns></returns>
        public string TypeOf()
        {
            return "MultyCalculation";
        }

        /// <summary>
        /// Получение массива с данными в формате JSON
        /// </summary>
        /// <returns></returns>
        public string GetJSON()
        {
           return JsonSerializer.Serialize(this.calculationList);
        }

        /// <summary>
        /// Очистка списка данных для отправления
        /// </summary>
        public void Clear()
        {
            this.calculationList.Clear();
        }

        /// <summary>
        /// Количество данных для отправления
        /// </summary>
        /// <returns></returns>
        public int Length()
        {
            return this.calculationList.Count;
        }

        public void Print()
        {
            foreach (var item in calculationList)
            {
                item.Print();
            }
        }
    }
}
