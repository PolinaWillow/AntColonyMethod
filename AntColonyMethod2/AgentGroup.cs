using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AntColonyLib;

namespace AntColonyMethod2
{
    public class AgentGroup
    {
        /// <summary>
        /// Монитор
        /// </summary>
        private object _monitor;

        /// <summary>
        /// Список агентов
        /// </summary>
        public List<Agent> Agents { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public AgentGroup()
        {
            _monitor = new object();
            Agents = new List<Agent>();
        }

        /// <summary>
        /// Добавление нового агента
        /// </summary>
        /// <param name="id">Id агента</param>
        public string AddNewAgent()
        {
            string id = Guid.NewGuid().ToString();
            Monitor.Enter(_monitor);

            Agent agent = new Agent { idAgent = id };
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

            Agent agent = Agents.FirstOrDefault(r => r.idAgent == id);
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
        public Agent FindAgent(string id)
        {
            Monitor.Enter(_monitor);

            Agent agent = Agents.FirstOrDefault(r => r.idAgent == id);
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

            for (int i = 0; i < dataTask.graf.paramCount; i++)
            {
                dataTask.graf.Params[way[i]].pheromones += delta;
            }

            return delta;
        }

        /// <summary>
        /// Изменение феромонов
        /// </summary>
        /// <param name="agent">Список агентов</param>
        /// <returns></returns>
        public int PheromoneEvaporation(List<Agent> agents, DataTask dataTask) //Испарение феромона
        {
            //Умножаем для максимума, делаим для минимума
            foreach (GrafParams grafElem in dataTask.graf.Params)
            {
                double Evaporation = ChangeableParams.L * Convert.ToDouble(grafElem.pheromones);

                foreach (Agent agentElem in agents)
                {
                    foreach (int wayElem in agentElem.wayAgent)
                    {
                        if (wayElem == grafElem.idParam)
                        {
                            Evaporation += (1 - ChangeableParams.L) * agentElem.delta;
                        }
                    }
                }
                grafElem.pheromones = Evaporation;
            }
            return 0;
        }
    }
}
