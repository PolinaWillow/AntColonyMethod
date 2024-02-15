using AntColonyExtLib.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel
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

        private double AddDelta(double functionValue, InputData inputData, List<int> way) {
            double eps = 0.0000000000000001;
            double delta = 100 / (functionValue + eps);

            for(int i=0;i<inputData.inputParams.Params.Count();i++) {
                foreach (var val in inputData.inputParams.Params[i].valuesParam) {
                    if (val.idValue == way[i]) {
                        val.pheromones += delta;
                    }
                }
            }
            
            return delta;
        }

        public int AddPheromones(InputData inputData) {
            
            foreach (var agent in this.Agents) {
                int[] way = new int[agent.wayAgent.Count()];
                for (int i=0;i< agent.wayAgent.Count();i++) {
                    way[i] = agent.wayAgent[i];
                }
                agent.delta = this.AddDelta(agent.funcValue, inputData, agent.wayAgent);
            }
            return 0;
        }

        public int PheromoneEvaporation(InputData inputData) {
            //Умножаем для максимума, делаим для минимума
            foreach (var elem in inputData.inputParams.Params)
            {
                foreach (var val in elem.valuesParam)
                {
                    double Evaporation = 0.9 * Convert.ToDouble(val.pheromones);
                    foreach (var agent in this.Agents) {
                        foreach (var node in agent.wayAgent) {
                            if (node == val.idValue) {
                                Evaporation += (1 - 0.9) * agent.delta;
                            }
                        }
                    }
                    val.pheromones = Evaporation;
                }
            }
            return 0;
        }
    }
}
