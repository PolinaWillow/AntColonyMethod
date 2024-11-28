using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.ClusterInteraction.Models
{
    public class StatusCommunication
    {
        /// <summary>
        /// Ствтус соединения с кластером
        /// </summary>
        public string Status { get; set; }
        public int timeDelayStatus {get; set;}
        public int threadAgentCount { get; set;}
        public StatusCommunication(string status="", int timeDelay=0, int threadAgentCount = 0) {
            this.Status = status;
            this.timeDelayStatus = timeDelay;
            this.threadAgentCount = threadAgentCount;
        }

        /// <summary>
        /// Изменение статуса
        /// </summary>
        /// <param name="newStatus"></param>
        public void Set(string newStatus) {
            this.Status = newStatus;
        }

        /// <summary>
        /// Получение типа данных кдасса
        /// </summary>
        /// <returns></returns>
        public string TypeOf()
        {
            return "StatusCommunication";
        }
    }
}
