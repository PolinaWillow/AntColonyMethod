using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Data.Common;

namespace AntColonyLib
{
    public class SenderData
    {
        /// <summary>
        /// Номер порта
        /// </summary>
        private int PORT { get; set; } = 0;

        /// <summary>
        /// IP адрес подключения
        /// </summary>
        private string IP { get; set; } = "";

        /// <summary>
        /// Файл конфигурации
        /// </summary>
        private string ConfigFile = "../../../../AntColonyLib/config/config.txt";

        public SenderData() {
            GetConfig();
        }

        /// <summary>
        /// Чтение файла конфигурации для получения порта и IP подключения
        /// </summary>
        /// <returns></returns>
        private int GetConfig()
        {
            using (var sr = new StreamReader(ConfigFile))
            {
                string port = sr.ReadLine();
                PORT = Convert.ToInt32(port);
                IP = sr.ReadLine();
            }
            //Console.WriteLine("PORT: " + PORT + " IP: " + IP);
            return 0;
        }

        /// <summary>
        /// Отправление данных для подсчета значения целевой функции
        /// </summary>
        /// <param name="path">Массив значений параметров</param>
        /// <param name="pathLength">Длина массива</param>
        /// <returns></returns>
        private int SendData(List<string[]> path)
        {
            try
            {
                //Определение точки подключения
                var EndPoint = new IPEndPoint(IPAddress.Parse(IP), PORT);
                //Создание сокета
                var newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //Подключение к серверу
                newSocket.Connect(EndPoint);

                // конвертируем данные в массив байтов
                //var messageBytes;


                newSocket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0;
        }

        
        
    }
}
