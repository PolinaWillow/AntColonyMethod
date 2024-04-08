using AntColonyExtLib.DataModel.Statistic;
using AntColonyExtLib.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel
{
    public class Agent
    {
        /// <summary>
        /// Идентификатор агента
        /// </summary>
        public string idAgent { get; set; }

        /// <summary>
        /// Дельта
        /// </summary>
        public double delta { get; set; }

        /// <summary>
        /// Путь Агента
        /// </summary>
        public List<int> wayAgent { get; set; }

        /// <summary>
        /// Значение функции агента
        /// </summary>
        public double funcValue { get; set; }


        public Agent() {
            wayAgent = new List<int>();
            funcValue = 0;
        }

        /// <summary>
        /// Определение пути агента
        /// </summary>
        /// <param name="inputData">Структура входных данных</param>
        /// <returns></returns>
        public int[] FindAgentWay(InputData inputData, StatisticsCollection statistic) {
            int[] wayAgent; //Искомый путь агента

            ///////Перенести выбор графа в Инпут дата
            if (inputData.changeFlag) {
                //Определяем путь агента при изменении в клоне
                wayAgent = inputData.cloneInputParams.FindWay();
            }
            else { wayAgent = inputData.inputParams.FindWay(); }

           
            //Определяем хэш агента
            Hash hash = new Hash();
            string hashWay = hash.GetHash(wayAgent);

            if (!inputData.hashTable.ContainsKey(hashWay))
            {
                hash.AddNewHash(hashWay, wayAgent, inputData); //Добавление нового ключа в таблицй                            
            }
            else
            {
                //Поиск нового пути
                int[] newWayAgent = FindAlternativeWay(inputData, wayAgent, hashWay, statistic);
                hashWay = hash.GetHash(newWayAgent);
                hash.AddNewHash(hashWay, wayAgent, inputData);
                Array.Copy(newWayAgent, 0, wayAgent, 0, inputData.inputParams.Params.Count());
            }
            
            //Обновление насыщения вершины
            for (int i=0;i< inputData.inputParams.Params.Count(); i++)
            {
                if (inputData.changeFlag)
                {
                    inputData.cloneInputParams.Params[i].UpdateSaturation(wayAgent[i], 1);
                }
                else { inputData.inputParams.Params[i].UpdateSaturation(wayAgent[i], 1); }
                
            }
            

            return wayAgent;
        }

        private int[] FindAlternativeWay(InputData inputData, int[] startWay, string hashWay, StatisticsCollection statistic) {
            int nomParam = 0; //Номер параметра
            int[] newWay = new int[inputData.inputParams.Params.Count()]; //Новый путь
            Hash hash = new Hash();
            Array.Copy(startWay, 0, newWay, 0, inputData.inputParams.Params.Count());

            while (inputData.hashTable.ContainsKey(hashWay) && (nomParam < inputData.inputParams.Params.Count()))
            {
                //Сохраняем количество переборов
                statistic.KolEnumI++;
                NextWay(newWay, nomParam, inputData.inputParams.Params[nomParam]);

                if (newWay[nomParam] == startWay[nomParam] && nomParam < inputData.inputParams.Params.Count())
                {
                    while (newWay[nomParam] == startWay[nomParam] && nomParam < (inputData.inputParams.Params.Count() - 1))
                    {
                        nomParam += 1;
                        NextWay(newWay, nomParam, inputData.inputParams.Params[nomParam]);
                    }
                    nomParam = 0;
                }
                hashWay = hash.GetHash(newWay);
            }

            return newWay;
        }

        /// <summary>
        /// Изменение вершины на слое
        /// </summary>
        /// <param name="way">Путь агента</param>
        /// <param name="nomParam">Номер слоя</param>
        /// <param name="Param">Слой</param>
        /// <returns></returns>
        private int NextWay(int[] way, int nomParam, Param Param)
        {
            //Увелечение значения параметра на 1
            way[nomParam] += 1;
            if (way[nomParam] > Param.valuesParam[Param.defParam.valuesCount-1].idValue) { way[nomParam] = Param.valuesParam[0].idValue; }

            return 0;
        }

        internal void Print()
        {
            Console.Write("id-" + this.idAgent + "Way: ");
            foreach (var elem in this.wayAgent) {
                Console.Write(elem+" ");
            }
            Console.WriteLine();
        }
    }
}
