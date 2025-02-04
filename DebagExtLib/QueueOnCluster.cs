using AntColonyExtLib.ClusterInteraction.Models.Calculation;
using AntColonyExtLib.DataModel;
using AntColonyExtLib.DataModel.DataForUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebagExtLib
{
    internal class QueueOnCluster
    {
        /// <summary>
        /// Очередь отправления на кластер
        /// </summary>
        public List<Calculation_v2> Queue { get; set; }

        private object _monitor { get; set; }

        public QueueOnCluster() {
            Queue = new List<Calculation_v2>();
            _monitor = new object();
        }

        /// <summary>
        /// Добавление в конец очереди
        /// </summary>
        public void AddToQueue(int[] way = null, InputData inputData = null, string idAgent=null)
        {
            Monitor.Enter(_monitor);

            Calculation_v2 calculation = new Calculation_v2(idAgent, way, inputData);
            Queue.Add(calculation);

            Monitor.Exit(_monitor);
        }

        /// <summary>
        /// Получение элемента очереди по принципу FIFO
        /// </summary>
        /// <returns></returns>
        public Calculation_v2 GetFromQueue()
        {
            Monitor.Enter(_monitor);

            if (this.Queue.Count == 0)
            {
                Monitor.Exit(_monitor);
                return null;
            }
            else
            {
                Calculation_v2 calculation = this.Queue[0];
                this.Queue.RemoveAt(0);
                Monitor.Exit(_monitor);
                return calculation;
            }
            
            
        }

        public void Print()
        {
            Console.WriteLine("\n\nОчередь");
            for (int i=0; i < Queue.Count; i++)
            {
                Console.WriteLine(i+":\tid: " + Queue[i].idAgent+"\tway: " + Queue[i].Way_For_Send.ToString());
            }
        }
    }
}
