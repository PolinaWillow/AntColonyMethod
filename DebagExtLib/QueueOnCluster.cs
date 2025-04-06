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

        private bool _endQueue { get; set; }

        private object _monitor { get; set; }

        public QueueOnCluster() {
            Queue = new List<Calculation_v2>();
            _endQueue = false;
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

        public void End()
        {
            _endQueue = true;
        }

        public bool IsEnd()
        {
            if (this._endQueue && this.Queue.Count==0) return true;
            return false;
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

        public List<Calculation_v2> GetAll() //Почему исключение, что кто-то изменил очередь, хотя поднят монитор???
        {
            Monitor.Enter(this._monitor);
            if (this.Queue.Count == 0)
            {
                Monitor.Exit(this._monitor);
                return null;
            }
            else
            {
                List<Calculation_v2> calculations = new List<Calculation_v2>();

                //calculations.AddRange(this.Queue);
                //this.Queue.Clear();
                foreach (Calculation_v2 item in this.Queue.ToList()) //foreach (Calculation_v2 item in this.Queue.ToList)
                {
                    calculations.Add(item);
                    this.Queue.Remove(item);
                }
                Monitor.Exit(this._monitor);
                return calculations;
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
