using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel
{
    public class AgentDictionary //Словарь агентов для из поиска при изменении ферамона
    {
        private object _monitor;

        Dictionary<string, Agent> dictionary { get; set; } //Словарь с агентами

        public bool Add(Agent agent)
        {
            Monitor.Enter(_monitor);
            if (this.dictionary.ContainsKey(agent.idAgent))
            {
                Monitor.Exit(_monitor);
                return false;
            }

            this.dictionary.Add(agent.idAgent, (Agent)agent.Clone()); //Сохраняем копию агента до изменения его ID
            Monitor.Exit(_monitor);
            return true;
        }
        
        public Agent Get(string id)
        {
            Monitor.Enter(_monitor);
            
            if (!this.dictionary.ContainsKey(id))
            {
                Monitor.Exit(_monitor);
                return null;
            }
            Agent agent = this.dictionary[id]; //Получаем агента

            //this.dictionary.Remove(id);//Удаляем агента

            Monitor.Exit(_monitor);
            return agent;
        }

        public bool Delete(string id)
        {
            Monitor.Enter(_monitor);
            if (!this.dictionary.ContainsKey(id))
            {
                Monitor.Exit(_monitor);
                return false;
            }
            this.dictionary.Remove(id);

            Monitor.Exit(_monitor);
            return true;
        }

        public void Clear()
        {
            this.dictionary.Clear();
        }

        public AgentDictionary()
        {
            _monitor = new object();
            this.dictionary = new Dictionary<string, Agent>();
        }
    }
}
