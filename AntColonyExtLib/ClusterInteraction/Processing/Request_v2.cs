using AntColonyExtLib.ClusterInteraction.Models;
using AntColonyExtLib.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AntColonyExtLib.ClusterInteraction.Processing
{
    public class Request_v2
    {
        public Sender request { get; set; }
        private Sender response { get; set; }

        private Socket socket { get; set; }

        public ClusterInfo clusterInfo { get; set; }
        public Request_v2()
        {
            clusterInfo = new ClusterInfo();
            request = new Sender();
            response = new Sender();

            //Создание сокета и открытие соединения
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Console.WriteLine("connected...");

        }

        /// <summary>
        /// Определение заблокирован ли сокет
        /// </summary>
        /// <returns></returns>
        public bool getSocketStatus()
        {
            if (this.socket.Blocking) return true; //Сокет заблокирован
            else return false; //Сокет сфободен
        }

        /// <summary>
        /// Закрытие соединения
        /// </summary>
        /// <returns></returns>
        public bool CloseConnect()
        {
            try
            {
                this.socket.Close();
                return true;
            }
            catch { return false; }
        }


        public Sender Post()
        {
            this.socket.Connect(this.clusterInfo.IP, this.clusterInfo.PORT);

            //Формирование строки Json для отправки данных
            string jsonData = JsonSerializer.Serialize(this.request);

            //Представление строки в виде байтов
            byte[] dataBytes = Encoding.Default.GetBytes(jsonData);

            //Отправка данных
            this.socket.Send(dataBytes);
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

            return this.response;
        }
    }
}
