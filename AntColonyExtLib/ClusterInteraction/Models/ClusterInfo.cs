using System.Net;

namespace AntColonyExtLib.ClusterInteraction.Models
{
    internal class ClusterInfo
    {
        /// <summary>
        /// Порт подключенич
        /// </summary>
        public int PORT { get; set; }
        /// <summary>
        /// IP адресс класстера
        /// </summary>
        public string IP { get; set; }

        public ClusterInfo()
        {
            PORT = 2000;

            //Получение IP:
            string host = Dns.GetHostName(); //Получение имени компьютера
            //Зщдучение адреса
            IPAddress address = Dns.GetHostAddresses(host).First<IPAddress>(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            if (address != null)
            {
                IP = address.ToString();
            }
        }

        public string TypeOf()
        {
            return "ClusterInfo";
        }
    }
}
