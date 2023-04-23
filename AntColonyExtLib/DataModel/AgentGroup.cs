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

        internal void AddWayAgent(int[] wayAgent, string id)
        {
            Monitor.Enter(_monitor);

            Agent agent = Agents.FirstOrDefault(r => r.idAgent == id);
            if (agent != null)
            {
                agent.wayAgent.AddRange(wayAgent);
            }

            Monitor.Exit(_monitor);
        }
    }
}
