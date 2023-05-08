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
    public class ClusterInteraction
    {
        private ClusterInfo SendInfo { get; set; }

        private SendData sendData { get; set; }

        private double outputClusterData { get; set; } 

        public ClusterInteraction(string typeFunction, int[] way, InputData inputData, double carentExtr = default(double)) {
            outputClusterData = 0;
            SendInfo = new ClusterInfo();
            sendData = new SendData(typeFunction, way, inputData, carentExtr);
            //JsonFile = new FileStream("way.json", FileMode.OpenOrCreate);


        }

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
