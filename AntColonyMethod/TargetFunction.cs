using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyMethod
{
    class TargetFunction
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
        private string ConfigFile = "../../../config/config.txt";

        /// <summary>
        /// Значение целефой функции
        /// </summary>
        public double valueTarget { get; set; }

        public TargetFunction()
        {
            GetConfig();
            valueTarget = 0;
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
        private int SendData(string[] path, int pathLength)
        {
            try
            {
                //Определение точки подключения
                var EndPoint = new IPEndPoint(IPAddress.Parse(IP), PORT);
                //Создание сокета
                var newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //Подключение к серверу
                newSocket.Connect(EndPoint);

                //Отправление длины массива
                string lengthMas = Convert.ToString(pathLength);
                byte[] data = Encoding.Unicode.GetBytes(lengthMas);
                newSocket.Send(data);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// Подсчет значения целефой функции
        /// </summary>
        /// <param name="way">Выбранный путь</param>
        /// <param name="graf">Граф</param>
        /// <param name="paramCount">Количество параметров</param>
        /// <returns></returns>
        public double FindValue(List<int> way, List<GrafParams> graf, int paramCount) //Подсчет целивой функции
        {
            double[] path = new double[paramCount - 1];
            for (int i = 0; i < paramCount - 1; i++)
            {
                path[i] = Convert.ToDouble(graf[way[i]].valueParam);
            }

            double Value = path[0] - path[1] + 2 * path[2] + path[3] + 2 * path[4] + 0.5 * path[5] - 0.12 * path[6] - path[7] + 80 * path[8] + 0.00001 * path[9];

            if (string.Compare(graf[way[10]].valueParam, "Сильное") == 0)
            {
                Value += 20;
            }
            return Value;
        }

        /// <summary>
        /// Поиск максимума функции
        /// </summary>
        /// <param name="dataTask">Набор исходных данных</param>
        /// <param name="agent">Проверяемый агент</param>
        /// <param name="max">Значене максимума</param>
        /// <param name="maxFunction">Массив для хранения максимума и пути его получения</param>
        /// <param name="wayAgent">Путь агента</param>
        /// <returns></returns>
        public double FindMaxFunction(DataTask dataTask, Agent agent, double max, string[] maxFunction, int[] wayAgent)
        {
            double valueFunction = FindValue(agent.wayAgent, dataTask.graf.Params, dataTask.paramCount); //Вычисление значений критериев
                                                                                                         //Console.WriteLine("Значение критерия: " + valueFunction + "\n");
            if (valueFunction >= max)
            {
                maxFunction[0] = Convert.ToString(valueFunction);
                for (int k = 1; k < dataTask.paramCount + 1; k++)
                {
                    maxFunction[k] = dataTask.graf.Params[wayAgent[k - 1]].valueParam;
                }
                max = valueFunction;
            }

            return max;
        }

        /// <summary>
        /// Поиск минимума функции
        /// </summary>
        /// <param name="dataTask">Набор исходных данных</param>
        /// <param name="agent">Проверяемый агент</param>
        /// <param name="min">Значене минимума</param>
        /// <param name="minFunction">Массив для хранения минимума и пути его получения</param>
        /// <param name="wayAgent">Путь агента</param>
        /// <returns></returns>
        public double FindMinFunction(DataTask dataTask, Agent agent, double min, string[] minFunction, int[] wayAgent)
        {
            double valueFunction = FindValue(agent.wayAgent, dataTask.graf.Params, dataTask.paramCount); //Вычисление значений критериев

            if (valueFunction <= min)
            {
                minFunction[0] = Convert.ToString(valueFunction);
                for (int k = 1; k < dataTask.paramCount + 1; k++)
                {
                    minFunction[k] = dataTask.graf.Params[wayAgent[k - 1]].valueParam;
                }
                min = valueFunction;
            }

            return min;
        }
    }
}
