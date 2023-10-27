using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel.ClusterData
{
    public class ClusterInfo
    {
        public int PORT { get; set; }
        public string IP { get; set; }

        public ClusterInfo() {
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
    }
}
