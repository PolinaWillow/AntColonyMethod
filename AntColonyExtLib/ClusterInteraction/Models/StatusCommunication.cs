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
        public StatusCommunication(string status="") {
            this.Status = status;
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
