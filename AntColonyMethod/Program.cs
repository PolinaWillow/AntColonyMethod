using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyMethod
{
    class Program
    {
        public static int ATTEMPTS_COUNT = 10000000; //Количество попыток генерации нового пути
        public static int MAX_ANT_COUNT = 25; //Максимальное количество муравьев

        static void Main(string[] args)
        {
            //Получение входных данных
            DataTask dataTask = new DataTask();
            DataReader dataReader = new DataReader();
            dataTask = dataReader.GettingInputData();

            //Создание графа  
            dataTask.graf.CreateGraf(dataTask);

            string[] maxFunction = new string[dataTask.paramCount + 1]; //Массив хранения максимума функции и значения параметров
            string[] minFunction = new string[dataTask.paramCount + 1]; //Массив хранения минимума функции и значения параметров

            double max = double.MinValue;
            double min = double.MaxValue;

            //Создание выходного файла
            DataWriter dataWriter = new DataWriter();
            string outputFile = dataWriter.CreateOutputFile(dataTask); 

            int attempt = 0; //Количество попыток нахождения уникального пути

            //Статистика
            StatisticsCollection statistics = new StatisticsCollection();
            string outputStat = dataWriter.CreateStatisticsOutputFile(statistics);

            //Варьирование количества муравьев
            while (dataTask.antCount < MAX_ANT_COUNT)
            {
                //Сброс статистики
                statistics.StartStatistics();

                while (statistics.LaunchesCount < statistics.numLaunches)
                {
                    //Переинициализация;
                    max = double.MinValue;
                    min = double.MaxValue;
                    dataTask.ResetDatatTask();
                    statistics.ResetStatistics();

                    //Прохождение всех итераций
                    for (int j = 0; j < dataTask.iterationCount; j++)
                    {
                        //Создание группы агентов
                        AgentGroup agentGroup = new AgentGroup(); //Список агентов

                        int CountAgent = 0;

                        //Прохождение K агентов
                        if (dataTask.availabilityThread)
                        {
                            Parallel.For(0, dataTask.antCount, (i, state) =>
                            {
                                AgentPassage(dataTask, agentGroup, CountAgent, attempt, min, max, maxFunction, minFunction, statistics);
                                max = Convert.ToDouble(maxFunction[0]);
                                min = Convert.ToDouble(minFunction[0]);
                            });
                        }
                        else
                        {
                            for (int i = 0; i < dataTask.antCount; i++)
                            {
                                AgentPassage(dataTask, agentGroup, CountAgent, attempt, min, max, maxFunction, minFunction, statistics);
                                max = Convert.ToDouble(maxFunction[0]);
                                min = Convert.ToDouble(minFunction[0]);
                                //Console.WriteLine("min = " + min);
                            }
                        }

                        TargetFunction targetFun = new TargetFunction();
                        //Занесение феромона
                        for (int i = 0; i < dataTask.antCount; i++)
                        {
                            double functionValue = targetFun.FindValue(agentGroup.Agents[i].wayAgent, dataTask.graf.Params, dataTask.paramCount);
                            agentGroup.Agents[i].delta = dataTask.graf.AddPheromone(dataTask, agentGroup.Agents[i].wayAgent, functionValue);
                        }

                        //Испарение феромонов
                        dataTask.graf.PheromoneEvaporation(agentGroup.Agents);

                        //Занесение результатов прохода итерации в файл
                        dataWriter.GettingOutputData(outputFile, (j + 1), maxFunction, minFunction);

                        //Сбор статистики после каждой итерации
                        statistics.UniqueSolutionCount = dataTask.antCount;
                        statistics.CollectingStat(j, statistics.UniqueSolutionCount);
                    }

                    //Запись статистики в файл
                    dataWriter.GettingOutputData(outputStat, statistics, dataTask);

                    statistics.LaunchesCount++;
                }

                dataTask.antCount += 5;
            }
        }

        public static void AgentPassage(DataTask dataTask, AgentGroup agentGroup, int CountAgent, int attempt, double min, double max, string[] maxFunction, string[] minFunction, StatisticsCollection statistics)
        {
            CountAgent++;

            string id = Guid.NewGuid().ToString();
            agentGroup.AddNewAgent(id);

            Agent agent = agentGroup.FindAgent(id);

            //Определение пути агента
            int[] wayAgent = agent.FindAgentWay_Method1(dataTask);
            //int[] wayAgent = agent.FindAgentWay_Method2(dataTask);
            
            attempt += 1;

            //Сохранение пути агента
            agentGroup.AddWayAgent(wayAgent, id);            

            //Поиск максимума и минимума
            TargetFunction targetFunction = new TargetFunction();

            //Сбор статистики о количестве найденных оптимумов
            List<int> way = new List<int>();
            way.AddRange(wayAgent);
            statistics.FindOptimalCount(targetFunction.FindValue(way, dataTask.graf.Params, dataTask.paramCount));


            targetFunction.FindMaxFunction(dataTask, agent, max, maxFunction, wayAgent);
            targetFunction.FindMinFunction(dataTask, agent, min, minFunction, wayAgent);

            // Сброс феромонов
            if (attempt == ATTEMPTS_COUNT)
            {
                dataTask.graf.InitialGraf();
                attempt = 0;
            }

        }
    }
}