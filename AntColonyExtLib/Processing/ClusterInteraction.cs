using AntColonyExtLib.DataModel;
using AntColonyExtLib.DataModel.ClusterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AntColonyExtLib.Processing
{
    /// <summary>
    /// Класс для взаимодействия с вычислительным кластером
    /// </summary>
    public class ClusterInteraction
    {
        /// <summary>
        /// Логи подключения
        /// </summary>
        private ClusterInfo SendInfo { get; set; }

        //Отправляемые данные
        private SendData sendData { get; set; }

        //Полученное значение с вычислительного класстера
        private double outputClusterData { get; set; } 

        public ClusterInteraction(string typeFunction="", int[] way =null, InputData inputData=null) {
            if (typeFunction != "")
            {
                outputClusterData = 0;
                sendData = new SendData(typeFunction, way, inputData);
            }
            SendInfo = new ClusterInfo();
            //JsonFile = new FileStream("way.json", FileMode.OpenOrCreate);
        }

        /// <summary>
        /// Функция начала и конца связи с кластером
        /// </summary>
        /// <param name="comand">start || end</param>
        /// <returns></returns>
        public bool InitCommunication(string comand = "start")
        {
            string jsonData = JsonSerializer.Serialize(comand);
            byte[] dataBytes = Encoding.Default.GetBytes(jsonData);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(this.SendInfo.IP, this.SendInfo.PORT);
            socket.Send(dataBytes);
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
            byte[] totalBytes = memoryStream.ToArray();
            memoryStream.Close();
            string readData = Encoding.Default.GetString(totalBytes);
            return JsonSerializer.Deserialize<bool>(readData);
        }

        /// <summary>
        /// Функция отправления данных
        /// </summary>
        /// <returns></returns>
        public ResultValueFunction SendWay() {
            //Формирование строки Json для отправки данных
            string jsonData = JsonSerializer.Serialize(this.sendData);
            //Console.WriteLine(jsonData);

            //Представление строки в виде байтов
            byte[] dataBytes = Encoding.Default.GetBytes(jsonData);

            //Создание сокета
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(this.SendInfo.IP, this.SendInfo.PORT);
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
            this.outputClusterData = JsonSerializer.Deserialize<double>(readData);
            //Console.WriteLine(outputClusterData);
            //Console.ReadKey();

            socket.Close();

            int length = this.sendData.Way_For_Send.Length;
            string[] valuesParam = new string[length];
            for (int i = 0; i < length; i++) {
                valuesParam[i] = this.sendData.Way_For_Send[i].SendValue;
            }

            ResultValueFunction curentValue = new ResultValueFunction();
            curentValue.Set(Convert.ToDouble(outputClusterData), valuesParam);
            return curentValue;//Convert.ToDouble(outputClusterData);
        }

    }
}
