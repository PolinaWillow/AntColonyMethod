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


        public Agent()
        {
            wayAgent = new List<int>();
            funcValue = 0;
        }

        public void AddWay(int[] wayAgent)
        {
            this.wayAgent.Clear();
            this.wayAgent.AddRange(wayAgent);
        }

        public object Clone()
        {
            Agent objClone = new Agent();
            objClone.idAgent = this.idAgent; 
            objClone.delta = this.delta;
            objClone.funcValue = this.funcValue;
            objClone.wayAgent = new List<int>();

            foreach (var item in this.wayAgent)
            {
                objClone.wayAgent.Add(item);
            }

            return objClone;
        }

        public string UpdateID()
        {
            string newId = Guid.NewGuid().ToString();

            this.idAgent = newId;
            return newId;
        }

        /// <summary>
        /// Определение пути агента
        /// </summary>
        /// <param name="inputData">Структура входных данных</param>
        /// <returns></returns>
        public int[] FindAgentWay(InputData inputData, bool hashTableStatus = true, HashStatistic hashStatistic=null, int iteration=0)
        {
            int[] wayAgent; //Искомый путь агента

            ///////Перенести выбор графа в Инпут дата
            if (inputData.changeFlag)
            {
                //Определяем путь агента при изменении в клоне
                wayAgent = inputData.cloneInputParams.FindWay();
            }
            else { wayAgent = inputData.inputParams.FindWay(); }


            if (hashTableStatus) //Если внедряем хэш-таблицы
            {
                Hash hash = new Hash();
                string hashWay = hash.GetHash(wayAgent);

                while (inputData.AddNewHash(hashWay, wayAgent) < 0) //Добавление нового ключа в таблицй  
                {
                    //Поиск нового пути
                    int[] newWayAgent = FindAlternativeWay(inputData, wayAgent, hashWay);
                    hashWay = hash.GetHash(newWayAgent);
                    //hash.AddNewHash(hashWay, wayAgent, inputData);
                    Array.Copy(newWayAgent, 0, wayAgent, 0, inputData.inputParams.Params.Count());

                    //Считаем сколько раз было непопадание по хэш
                    hashStatistic.Count(iteration);
                }

                //Обновление насыщения вершины
                for (int i = 0; i < inputData.inputParams.Params.Count(); i++)
                {
                    if (inputData.changeFlag)
                    {
                        inputData.cloneInputParams.Params[i].UpdateSaturation(wayAgent[i], 1);
                    }
                    else { inputData.inputParams.Params[i].UpdateSaturation(wayAgent[i], 1); }

                }

            }
            //else
            //{
            //    inputData.AddNewHash(hashWay, wayAgent);
            //}


            return wayAgent;
        }

        private int[] FindAlternativeWay(InputData inputData, int[] startWay, string hashWay)
        {
            int nomParam = 0; //Номер параметра
            int[] newWay = new int[inputData.inputParams.Params.Count()]; //Новый путь
            Hash hash = new Hash();
            Array.Copy(startWay, 0, newWay, 0, inputData.inputParams.Params.Count());

            while (inputData.hashTable.ContainsKey(hashWay) && (nomParam < inputData.inputParams.Params.Count()))
            {
                //Сохраняем количество переборов
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
            if (way[nomParam] > Param.valuesParam[Param.defParam.valuesCount - 1].idValue) { way[nomParam] = Param.valuesParam[0].idValue; }

            return 0;
        }

        /// <summary>
        /// Изменение феромона агентом (Добавление и испаврение)
        /// </summary>
        /// <param name="inputData">Граф путей</param>
        /// <returns></returns>
        public int ChangePheromones(InputData inputData)
        {
            //Добавление ферамонов
            int[] way = new int[this.wayAgent.Count()];
            for (int i = 0; i < this.wayAgent.Count(); i++)
            {
                way[i] = this.wayAgent[i];
            }
            this.delta = this.AddDelta(inputData);

            //Испаврение ферамонов на графе
            //Умножаем для максимума, делаим для минимума
            foreach (var elem in inputData.cloneInputParams.Params)
            {
                foreach (var val in elem.valuesParam)
                {
                    double Evaporation = 0.9 * Convert.ToDouble(val.pheromones);

                    foreach (var node in this.wayAgent)
                    {
                        if (node == val.idValue)
                        {
                            Evaporation += (1 - 0.9) * this.delta;
                        }
                    }

                    val.pheromones = Evaporation;
                }
            }

            //Смена флага изменений
            inputData.changeFlag = true;

            return 0;
        }

        private double AddDelta(InputData inputData)
        {
            double eps = 0.0000000000000001;
            double delta = 100 / (this.funcValue + eps);

            for (int i = 0; i < inputData.cloneInputParams.Params.Count(); i++)
            {
                foreach (var val in inputData.cloneInputParams.Params[i].valuesParam)
                {
                    if (val.idValue == this.wayAgent[i])
                    {
                        val.pheromones += delta;
                    }
                }
            }

            return delta;
        }

        internal void Print()
        {
            Console.Write("id-" + this.idAgent + "Way: ");
            foreach (var elem in this.wayAgent)
            {
                Console.Write(elem + " ");
            }
            Console.WriteLine();
        }
    }
}
