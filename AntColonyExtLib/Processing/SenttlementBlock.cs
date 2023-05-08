using AntColonyExtLib.DataModel;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.Processing
{
    public class SenttlementBlock
    {
        public SenttlementBlock() {
            results = new List<string>();
            _monitor = new object();
            maxFunction = new ResultValueFunction();
            MAX = 0;
        }
        public static List<string> results  { get;set;}

        static ResultValueFunction maxFunction { get; set; }
        static double MAX { get; set; }

        private object _monitor;

        public string OutputResults()
        {
            while (true)
            {
                Monitor.Enter(_monitor); // ждем, пока не будет доступа к вызывающей функции
                if (results.Count > 0)
                {
                    string result = results[0];
                    results.RemoveAt(0);
                    return result;
                }
                Monitor.Exit(_monitor); // освобождаем доступ к вызывающей функции
            }
        }

        public void OutputResultToCaller(string result) {
            Monitor.Enter(_monitor);
            results.Add(result); // добавляем промежуточный результат в список
            Monitor.Exit(_monitor);
        }

        public int Senttlement(InputData inputData, CancellationToken cancelToken)
        {
            //Console.WriteLine("Начало расчета");
            int countFindWay=0; //Количество найденных путей
            int countAgent = 0; //Количество пройденных агентов
            
            FileManager fileManager = new FileManager(); //Менеджер файлов для занесения результатов
            //Создание файла с выходными результатами
            string outputFileName = fileManager.CreateOutputFile(inputData);
            

            //Проход по всем итерациям
            for (int i = 0; i < inputData.iterationCount; i++) {
                //Console.WriteLine(cancelToken);
                if (countFindWay == inputData.inputParams.countCombinationsV) {
                    //Console.WriteLine("EXIT-1: Все пути найдены") ;
                    return 0;
                }
                
                //Создание группы агентов
                AgentGroup agentGroup = new AgentGroup();
                //Прохождение K агентов
                //Console.WriteLine ("Текущий максимум: ");
                maxFunction.Print();
                for (int j = 0; j < inputData.antCount; j++) {
                    AgentPassage(inputData, countFindWay, countAgent, agentGroup);
                }

                //Занесение феромонов
                agentGroup.AddPheromones(inputData);
                //Испарение феромонов
                agentGroup.PheromoneEvaporation(inputData);

                //Занесение результатов прохода агентов в выходной файл
                string dataToOutput = fileManager.CreateWriteString(i, "max", maxFunction);
                fileManager.Write(outputFileName, dataToOutput);

                //--------------------------------------------
                //ВЫВОД ПРОМЕЖУТОЧНЫХ РЕЗУЛЬТАТОВ ПОЛЬЗОВАТЕЛЮ
                //results.Add(dataToOutput);
                OutputResultToCaller(dataToOutput);

                //--------------------------------------------

                //Проверка на появление события останова расчетов
                //Проверка токена
                if (cancelToken.IsCancellationRequested) {
                    //Console.WriteLine("Отмена выполнения по сигналу от пользователя");
                    return 0;
                }

                //Удаление агентов
                agentGroup.Agents.Clear();
            }
            
            return 0;
        }

        private static void AgentPassage(InputData inputData, int countFindWay,int countAgent, AgentGroup agentGroup) {
            //Проверка просмотрены ли все решения:
            if (countFindWay == inputData.inputParams.countCombinationsV)
            {
                //Console.WriteLine("EXIT-2: Все пути найдены");
                return;
            }
            //Создание нового агента
            countAgent++;
            string id = agentGroup.AddNewAgent();
            Agent agent = agentGroup.FindAgent(id);

            //Определение пути агента
            int[] wayAgent = agent.FindAgentWay(inputData);
            agentGroup.AddWayAgent(wayAgent, id);
            countFindWay++;


            //Получение значения целевой функции и определение текущего найденного максимума
            ClusterInteraction clusterInteractionMax = new ClusterInteraction("val", wayAgent, inputData);
            ResultValueFunction valFunction = clusterInteractionMax.SendWay();
            valFunction.Print();

            if (valFunction.valueFunction >= MAX) {
                maxFunction=valFunction;
                MAX = valFunction.valueFunction;
            }

            //Проверка просмотрены ли все решения:
            if (countFindWay == inputData.inputParams.countCombinationsV)
            {
                //Console.WriteLine("EXIT-3: Все пути найдены");
                return;
            }
        }
    }
}
