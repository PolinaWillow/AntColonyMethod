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
        private object _monitor;
        public List<Agent> Agents { get; set; }

        public AgentGroup() {
            _monitor = new object();
            Agents = new List<Agent>();
        }

        public void AddNewAgent(int id) {
            Monitor.Enter(_monitor);

            Agent agent = new Agent { idAgent = id };
            Agents.Add(agent);

            Monitor.Exit(_monitor);
        }

        public void AddWayAgent(int[] wayAgent, int id) {
            Monitor.Enter(_monitor);

            Agent agent = Agents.FirstOrDefault(r => r.idAgent == id);
            if (agent != null) {
                agent.wayAgent.AddRange(wayAgent);
            }

            Monitor.Exit(_monitor);
        }

        public Agent FindAgent(int id) {
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
