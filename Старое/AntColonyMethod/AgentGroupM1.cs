using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AntColonyLib;
using AntColonyLib.DataModel;

namespace AntColonyMethod
{
    public class AgentGroupM1
    {
        /// <summary>
        /// Монитор
        /// </summary>
        private object _monitor;

        /// <summary>
        /// Список агентов
        /// </summary>
        public List<AgentM1> Agents { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public AgentGroupM1()
        {
            _monitor = new object();
            Agents = new List<AgentM1>();
        }

        /// <summary>
        /// Добавление нового агента
        /// </summary>
        /// <param name="id">Id агента</param>
        public string AddNewAgent()
        {
            string id = Guid.NewGuid().ToString();
            Monitor.Enter(_monitor);

            AgentM1 agent = new AgentM1 { idAgent = id };
            Agents.Add(agent);

            Monitor.Exit(_monitor);
            return id;

        }

        /// <summary>
        /// Добавление пути агента
        /// </summary>
        /// <param name="wayAgent">Путь агента</param>
        /// <param name="id">Id агента</param>
        public void AddWayAgent(int[] wayAgent, string id)
        {
            Monitor.Enter(_monitor);

            AgentM1 agent = Agents.FirstOrDefault(r => r.idAgent == id);
            if (agent != null)
            {
                agent.wayAgent.AddRange(wayAgent);
            }

            Monitor.Exit(_monitor);
        }

        /// <summary>
        /// Поиск агента по Id
        /// </summary>
        /// <param name="id">Id агента</param>
        /// <returns></returns>
        public AgentM1 FindAgent(string id)
        {
            Monitor.Enter(_monitor);

            AgentM1 agent = Agents.FirstOrDefault(r => r.idAgent == id);
            Monitor.Exit(_monitor);
            if (agent != null)
            {
                return agent;
            }

            return null;
        }

        /// <summary>
        /// Добавление феромонов
        /// </summary>
        /// <param name="dataTask">Набор исходных данных</param>
        /// <param name="way">Путь агента</param>
        /// <param name="function">Значение искомой функции</param>
        /// <returns></returns>
        public double AddPheromone(DataTask dataTask, List<int> way, double functionValue) //Добавление феромонов
        {
            double eps = 0.0000000000000001;
            double delta = ChangeableParams.Q / (functionValue + eps);

            for (int i = 0; i < dataTask.graphWorkCopy.paramCount; i++)
            {
                dataTask.graphWorkCopy.Params[way[i]].pheromones += delta;
            }

            return delta;
        }

        /// <summary>
        /// Изменение феромонов
        /// </summary>
        /// <param name="agent">Список агентов</param>
        /// <returns></returns>
        public int PheromoneEvaporation(List<AgentM1> agents, DataTask dataTask) //Испарение феромона
        {
            //Умножаем для максимума, делаим для минимума
            foreach (GraphParams graphElem in dataTask.graphWorkCopy.Params)
            {
                double Evaporation = ChangeableParams.L * Convert.ToDouble(graphElem.pheromones);

                foreach (AgentM1 agentElem in agents)
                {
                    foreach (int wayElem in agentElem.wayAgent)
                    {
                        if (wayElem == graphElem.idParam)
                        {
                            Evaporation += (1 - ChangeableParams.L) * agentElem.delta;
                        }
                    }
                }
                graphElem.pheromones = Evaporation;
            }
            return 0;
        }
    }
}
