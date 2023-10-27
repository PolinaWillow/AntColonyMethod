using AntColonyExtLib.DataModel;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntColonyExtLib.DataModel.Statistic;
using AntColonyExtLib.DataModel.DataForUser;

namespace AntColonyExtLib.Processing
{
    public class SenttlementBlock
    {
        public SenttlementBlock() {
            ResultsForUser = new StreamlinedData();
            //ResultsForUser.Notify += ResultsForUser_Notify;
            maxFunction = new ResultValueFunction();
            MAX = 0;
        }

        public StreamlinedData ResultsForUser  { get;set;}

        static ResultValueFunction maxFunction { get; set; }
        static double MAX { get; set; }

        //public string OutputResults()
        //{
        //    while (true)
        //    {
        //        MessageForUser res = ResultsForUser.GetMessage();
        //        if (res!=null) {
        //            //Передача данных пользователю
        //        }
        //    }
        //}

        public int Senttlement(string fileDataName, InputData inputData, CancellationToken cancelToken)
        {
            int countFindWay=0; //Количество найденных путей
            int countAgent = 0; //Количество пройденных агентов
            
            FileManager fileManager = new FileManager(); //Менеджер файлов для занесения результатов
            //Создание файла с выходными результатами
            string outputFileName = fileManager.CreateOutputFile(inputData, fileDataName);

            //Объявление сбора статистики
            StatisticsCollection statistics = new StatisticsCollection();
            string outputStat = fileManager.CreateStatisricFile();


            //Сбор статистики
            statistics.StartStatistics();

            //Сброс статистики по запуску
            statistics.ResetStatistics();

            //Определение времени начала расчета
            statistics.TimeStart=DateTime.Now;

            //Отправление запроса ксластеру на начало работы


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
                for (int j = 0; j < inputData.antCount; j++) {
                    AgentPassage(i, inputData, countFindWay, countAgent, agentGroup, statistics);
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
                //Определение процента расчетов
                long percentage =Convert.ToInt64(Convert.ToDouble(100)/ Convert.ToDouble(inputData.iterationCount) * Convert.ToDouble(i));
                ResultsForUser.AddToList(dataToOutput, percentage);

                //--------------------------------------------

                //Проверка на появление события останова расчетов
                //Проверка токена
                if (cancelToken.IsCancellationRequested) {
                    //Console.WriteLine("Отмена выполнения по сигналу от пользователя");
                    return 0;
                }

                //Удаление агентов
                agentGroup.Agents.Clear();

                //Сбор статистики по каждой терации
                statistics.UniqueSolutionCount = inputData.antCount;
                statistics.CollectingStat(i, statistics.UniqueSolutionCount);
            }

            //Определение времени выполнения
            statistics.TimeEnd = DateTime.Now;
            statistics.WorkTimeLaunch();
            //Сбор статистики о среднем числе переборов за итерацию
            statistics.EmunStatI(inputData.iterationCount);
            //Запись статистики в файл
            fileManager.WriteStatstring(outputStat, statistics, inputData);
            statistics.LaunchesCount++;

            return 0;
        }

        private static void AgentPassage(int NumIteration, InputData inputData, int countFindWay,int countAgent, AgentGroup agentGroup, StatisticsCollection statistics) {
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
            int[] wayAgent = agent.FindAgentWay(inputData, statistics);
            agentGroup.AddWayAgent(wayAgent, id);
            countFindWay++;


            //Получение значения целевой функции и определение текущего найденного максимума
            ClusterInteraction clusterInteractionMax = new ClusterInteraction("findValue", wayAgent, inputData);
            ResultValueFunction valFunction = clusterInteractionMax.SendWay();
            agent.funcValue = valFunction.valueFunction;
            //valFunction.Print();

            if (valFunction.valueFunction >= MAX) {
                maxFunction=valFunction;
                MAX = valFunction.valueFunction;
            }

            //Получение статистики о попадании в % от ожидаемого решения
            statistics.FindOptimalCount(valFunction.valueFunction, (NumIteration+1), agentGroup.Agents.Count());

            //Проверка просмотрены ли все решения:
            if (countFindWay == inputData.inputParams.countCombinationsV)
            {
                //Console.WriteLine("EXIT-3: Все пути найдены");
                return;
            }
        }
    }
}
