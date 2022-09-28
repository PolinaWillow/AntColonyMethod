using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntColonyMethod
{
    class AgentGroup
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
        public AgentGroup() {
            _monitor = new object();
            Agents = new List<Agent>();
        }

        /// <summary>
        /// Добавление нового агента
        /// </summary>
        /// <param name="id">Id агента</param>
        public string AddNewAgent() {
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
        public void AddWayAgent(int[] wayAgent, string id) {
            Monitor.Enter(_monitor);

            Agent agent = Agents.FirstOrDefault(r => r.idAgent == id);
            if (agent != null) {
                agent.wayAgent.AddRange(wayAgent);
            }

            Monitor.Exit(_monitor);
        }

        /// <summary>
        /// Поиск агента по Id
        /// </summary>
        /// <param name="id">Id агента</param>
        /// <returns></returns>
        public Agent FindAgent(string id) {
            Monitor.Enter(_monitor);

            Agent agent = Agents.FirstOrDefault(r => r.idAgent == id);
            Monitor.Exit(_monitor);
            if (agent != null)
            {               
                return agent;
            }
           
            return null;            
        }
    }
}
