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

        //Флаг открытости соединения
        private bool isConnected = false;

        public Request_v2()
        {
            clusterInfo = new ClusterInfo();
            request = new Sender();
            response = new Sender();
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
        /// Открытие соединения и отправление команды Start
        /// </summary>
        /// <param name="timeDelay">Задержка на кластере в мс</param>
        /// <param name="threadAgentCount">Количиство потоков агентов</param>
        /// <returns></returns>
        public bool Start(int timeDelay=0, int threadAgentCount = 1, int PakegCount=1)
        {
            if (!isConnected)
            {
                //Создание сокета и открытие соединения
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.socket.Connect(this.clusterInfo.IP, this.clusterInfo.PORT);
                //Console.WriteLine("connected...");
                this.isConnected = true;

                //Отправляем команду "start";
                StatusCommunication statusCommunication = new StatusCommunication("start", timeDelay, threadAgentCount, PakegCount);
                try
                {
                    this.request.AddData(statusCommunication.TypeOf(), JsonSerializer.Serialize(statusCommunication));
                    this.request.Print();
                    this.Post();
                    Console.WriteLine("Отправлен запрос Start");
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e + "Ошибка связи с кластером при отправлении statusCommunication = start");
                    this.socket.Close();
                    this.isConnected = false;
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Разрыв соединения с отправлением соответствующего запроса кластеру
        /// </summary>
        /// <returns></returns>
        public bool End()
        {
            if (isConnected)
            {
                //Отправляем запрос на окончание работы
                StatusCommunication statusCommunication = new StatusCommunication("end");

                try
                {
                    this.request.AddData(statusCommunication.TypeOf(), JsonSerializer.Serialize(statusCommunication));
                    this.request.Print();
                    this.Post();

                    socket.Close();
                    isConnected = false;

                    Console.WriteLine("Отправлен запрос End");
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e + "Ошибка связи с кластером при отправлении statusCommunication = end");
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Получение информации с вычислительного кластера для формирования графа
        /// </summary>
        public Sender GetData()
        {
            if (!this.isConnected) throw new InvalidOperationException("Соединение не установлено");

            this.request.AddData("GetGraphInfo", ""); //Заполняем запрос

            string jsonData = JsonSerializer.Serialize(this.request); //Запрос и отправляем его по сокету
            byte[] dataBytes = Encoding.Default.GetBytes(jsonData); //Представление строки в виде байтов
            this.socket.Send(dataBytes); //Отправка данных
           
            //Получение результата расчетов кластера
            byte[] buffer = new byte[1024 * 4];
            int readBytes = socket.Receive(buffer);
            MemoryStream memoryStream = new MemoryStream();

            while (readBytes > 0)
            {
                memoryStream.Write(buffer, 0, readBytes);
                if (socket.Available > 0) readBytes = socket.Receive(buffer);
                else break;
            }
            byte[] totalBytes = memoryStream.ToArray();
            memoryStream.Close();

            string readData = Encoding.Default.GetString(totalBytes);
            this.response = JsonSerializer.Deserialize<Sender>(readData);

            return this.response;
        }

        public Sender Post()
        {
            if (!this.isConnected) throw new InvalidOperationException("Соединение не установлено");
            
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
