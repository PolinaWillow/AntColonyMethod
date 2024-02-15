using AntColonyExtLib.ClusterInteraction.Models;
using AntColonyExtLib.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AntColonyExtLib.ClusterInteraction.Processing
{
    public class Request
    {
        public Sender request { get; set; }
        private Sender response { get; set; }

        public ClusterInfo clusterInfo { get; set; }
        public Request() {
            clusterInfo = new ClusterInfo();
            request = new Sender();
            response = new Sender();    
        }

        public Sender Post()
        {
            //Формирование строки Json для отправки данных
            string jsonData = JsonSerializer.Serialize(this.request);
            //onsole.WriteLine(this.request.Body);
            //Console.WriteLine(jsonData);

            //Представление строки в виде байтов
            byte[] dataBytes = Encoding.Default.GetBytes(jsonData);

            //Создание сокета
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(this.clusterInfo.IP, this.clusterInfo.PORT);
            //Console.WriteLine("connected...");

            //Отправка данных
            socket.Send(dataBytes);
            //Console.WriteLine("sent...");

            //Получение результата расчетов кластера
            byte[] buffer = new byte[1024 * 4];
            int readBytes = socket.Receive(buffer);
            MemoryStream memoryStream = new MemoryStream();

            while (readBytes > 0)
            {
                memoryStream.Write(buffer, 0, readBytes);
                if (socket.Available > 0)
                {
                    readBytes = socket.Receive(buffer);
                }
                else
                {
                    break;
                }
            }
            //Console.WriteLine("read...");
            byte[] totalBytes = memoryStream.ToArray();
            memoryStream.Close();


            string readData = Encoding.Default.GetString(totalBytes);
            //Console.WriteLine(readData);
            this.response = JsonSerializer.Deserialize<Sender>(readData);
            //Console.WriteLine(outputClusterData);
            //Console.ReadKey();

            socket.Close();
            return this.response;
        }
    }
}
