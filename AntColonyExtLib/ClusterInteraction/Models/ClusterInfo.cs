using System.Net;

namespace AntColonyExtLib.ClusterInteraction.Models
{
    public class ClusterInfo
    {
        /// <summary>
        /// Порт подключенич
        /// </summary>
        public int PORT { get; set; }
        /// <summary>
        /// IP адресс класстера
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Параметр для запуска на удаленном класетер
        /// </summary>
        bool isRemoteConnection = false;
        
        public ClusterInfo()
        {
            PORT = 2000;

            if (isRemoteConnection)
            {
                IP = "192.168.1.15";
            }
            else
            {
                //Получение IP:
                string host = Dns.GetHostName(); //Получение имени компьютера
                                                 //Зщдучение адреса
                IPAddress address = Dns.GetHostAddresses(host).First<IPAddress>(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                if (address != null)
                {
                    IP = address.ToString();
                }
            }
        }

        public string TypeOf()
        {
            return "ClusterInfo";
        }
    }
}
