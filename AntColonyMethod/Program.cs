using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AntColonyLib;

namespace AntColonyMethod
{
    class Program
    {
        static void Main(string[] args)
        {
            //Получение входных данных
            DataTask dataTask = new DataTask();
            DataReader dataReader = new DataReader();
            dataTask = dataReader.GettingInputData();

            //Создание графа  
            dataTask.CreateGraph();
            dataTask.CreateGraphClone("NoSort", 0);
           
            string[] maxFunction = new string[dataTask.graphWorkCopy.paramCount + 1]; //Массив хранения максимума функции и значения параметров
            string[] minFunction = new string[dataTask.graphWorkCopy.paramCount + 1]; //Массив хранения минимума функции и значения параметров

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
            while (dataTask.antCount < ChangeableParams.MAX_ANT_COUNT)
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

                    //Начала отсчета выполнения
                    statistics.TimeStart = DateTime.Now;

                    //Прохождение всех итераций
                    for (int j = 0; j < dataTask.iterationCount; j++)
                    {
                        //Если все решения просмотрениы, а итерации еще не закончены
                        if (dataTask.hashTable.Count == dataTask.controlCount)
                        {
                            Console.WriteLine("Все пути найдены, выполнение алгоритма остановлено 2");
                            break;
                        }
                        //Создание группы агентов
                        AgentGroup agentGroup = new AgentGroup(); //Список агентов

                        int CountAgent = 0;

                        //Прохождение K агентов
                        if (dataTask.availabilityThread)
                        {
                            Parallel.For(0, dataTask.antCount, (i, state) =>
                            {
                                AgentPassage(dataTask, agentGroup, CountAgent, attempt, min, max, maxFunction, minFunction, statistics, j);
                                max = Convert.ToDouble(maxFunction[0]);
                                min = Convert.ToDouble(minFunction[0]);                                
                            });
                        }
                        else
                        {
                            for (int i = 0; i < dataTask.antCount; i++)
                            {
                                AgentPassage(dataTask, agentGroup, CountAgent, attempt, min, max, maxFunction, minFunction, statistics, j);
                                max = Convert.ToDouble(maxFunction[0]);
                                min = Convert.ToDouble(minFunction[0]);                                                             
                            }
                        }

                        TargetFunction targetFun = new TargetFunction();
                        //Занесение феромона
                        for (int i = 0; i < agentGroup.Agents.Count(); i++)
                        {
                            double functionValue = targetFun.FindValue(agentGroup.Agents[i].wayAgent, dataTask.graphWorkCopy.Params, dataTask.graphWorkCopy.paramCount);
                            agentGroup.Agents[i].delta = agentGroup.AddPheromone(dataTask, agentGroup.Agents[i].wayAgent, functionValue);
                        }

                        //Испарение феромонов
                        agentGroup.PheromoneEvaporation(agentGroup.Agents, dataTask);

                        //Занесение результатов прохода итерации в файл
                        dataWriter.GettingOutputData(outputFile, (j + 1), maxFunction, minFunction);
                        
                        //Сбор статистики после каждой итерации
                        statistics.UniqueSolutionCount = dataTask.antCount;
                        statistics.CollectingStat(j, statistics.UniqueSolutionCount);

                        agentGroup.Agents.Clear();
                    }

                    //Конец отсчета выполнения
                    statistics.TimeEnd = DateTime.Now;
                    statistics.WorkTimeLaunch();

                    //Сбор статистки о среднем числе переборов за итерацию
                    statistics.EmunStatI(dataTask.iterationCount);

                    //Запись статистики в файл
                    dataWriter.GettingOutputData(outputStat, statistics, dataTask);                 

                    statistics.LaunchesCount++;
                }

                dataTask.antCount += ChangeableParams.ANT_INTERVAL;
            }
        }

        public static void AgentPassage(DataTask dataTask, AgentGroup agentGroup, int CountAgent, int attempt, double min, double max, string[] maxFunction, string[] minFunction, StatisticsCollection statistics, int nomIteration)

        {
            //Если все решения просмотрениы, а итерации еще не закончены
            if (dataTask.hashTable.Count == dataTask.controlCount)
            {
                Console.WriteLine("Все пути найдены, выполнение алгоритма остановлено 3");
                return;
            }

            CountAgent++;

            //Создание нового агента
            string id = agentGroup.AddNewAgent();
            Agent agent = agentGroup.FindAgent(id);

            //Определение пути агента
            int[] wayAgent = agent.FindAgentWay_Method(dataTask, statistics);

            attempt += 1;

            //Сохранение пути агента
            agentGroup.AddWayAgent(wayAgent, id);            

            //Поиск максимума и минимума
            TargetFunction targetFunction = new TargetFunction();

            //Сбор статистики о количестве найденных оптимумов
            List<int> way = new List<int>();
            way.AddRange(wayAgent);
            statistics.FindOptimalCount(targetFunction.FindValue(way, dataTask.graphWorkCopy.Params, dataTask.graphWorkCopy.paramCount), (nomIteration+1), agentGroup.Agents.Count());


            targetFunction.FindMaxFunction(dataTask, agent.wayAgent, max, maxFunction, wayAgent);
            targetFunction.FindMinFunction(dataTask, agent.wayAgent, min, minFunction, wayAgent);

            //Если все решения просмотрениы, а итерации еще не закончены
            if (dataTask.hashTable.Count == dataTask.controlCount)
            {
                Console.WriteLine("Все пути найдены, выполнение алгоритма остановлено 1");
                return;
            }

            // Сброс феромонов
            if (attempt == ChangeableParams.ATTEMPTS_COUNT)
            {
                dataTask.graphWorkCopy.InitialGraph();
                attempt = 0;
            }            
        }
    }
}