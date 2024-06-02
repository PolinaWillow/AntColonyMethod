using AntColonyExtLib.ClusterInteraction.Models;
using AntColonyExtLib.ClusterInteraction.Models.Calculation;
using AntColonyExtLib.ClusterInteraction.Processing;
using AntColonyExtLib.DataModel;
using AntColonyExtLib.DataModel.Statistic;
using AntColonyExtLib.Processing;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DebagExtLib
{
    internal class Program
    {
        public StreamlinedData ResultsForUser = new StreamlinedData();

        static ResultValueFunction maxFunction = new ResultValueFunction();
        static double MAX = 0;

        static async Task Main()
        {
            //Создание тестового объекта данных
            InputDataModel inputData = new InputDataModel();

            //Клонируем исходный шраф
            inputData.testInputData.cloneInputParams = (ParamsList)inputData.testInputData.inputParams.Clone();

            Console.WriteLine("---------cloneInputParams----------");
            inputData.testInputData.cloneInputParams.Print();
            Console.WriteLine("---------cloneInputParams----------");

            inputData.Print();

            //Создание имени выходного файла
            FileManager fileManager = new FileManager();
            string fileName = fileManager.CreateFileName("OutPutData");

            //Запуск расчетного блока
            await AsyncSenttlement(fileName, inputData.testInputData);
            //SyncSenttlement(fileName, inputData.testInputData);
        }

        public static async Task AsyncSenttlement(string fileDataName, InputData inputData)
        {
            int countFindWay = 0; //Количество найденных путей
            int countAgent = 0; //Количество пройденных агентов

            FileManager fileManager = new FileManager(); //Менеджер файлов для занесения результатов
            string outputFileName = fileManager.CreateOutputFile(inputData, fileDataName); //Создание файла с выходными результатами

            //Объявление сбора статистики
            StatisticsCollection statistics = new StatisticsCollection();
            string outputStat = fileManager.CreateStatisricFile();

            statistics.StartStatistics(); //Сбор статистики
            statistics.ResetStatistics();  //Сброс статистики по запуску    
            statistics.TimeStart = DateTime.Now;  //Определение времени начала расчета

            TimeStatistic timer = new TimeStatistic("TimeStatistic");
            timer.TimeStatistic_Start("all");


            //Отправление кластеру запрос на подтверждение
            Request_v2 reqCommunication = new Request_v2();
            StatusCommunication statusCommunication = new StatusCommunication("start");
            try
            {
                Console.WriteLine("Отправлен запрос на начало сессии");
                reqCommunication.request.AddData(statusCommunication.TypeOf(), JsonSerializer.Serialize(statusCommunication.Status));
                reqCommunication.Post();
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "Ошибка связи с кластером при отправлении statusCommunication = start");
            }

            ///Создаем очередь и группу агентов
            QueueOnCluster queueOnCluster = new QueueOnCluster();
            AgentGroup agentGroup = new AgentGroup();
            
            //Определение путей агентов (Параллельный поиск)
            Task FindWayTask = Task.Run(() =>
            {
                timer.TimeStatistic_Start("findWayTask");

                int countFindWay = 0; //Количество найденных путей
                int i = 0;

                while (countFindWay < inputData.inputParams.countCombinationsV)
                {
                    i++;

                    List<string> newAgentsList = new List<string>();
                    //Создаем группу агентов
                    for (int j = 0; j < 2; j++)
                    {
                        string id = agentGroup.AddNewAgent();
                        newAgentsList.Add(id);
                    }

                    List<Task> tasks = new List<Task>();
                    foreach (string agentID in newAgentsList)
                    {
                        tasks.Add(Task.Factory.StartNew(() => {
                            //Если просмотрены все пути, то выход из задачи
                            if (countFindWay < inputData.inputParams.countCombinationsV)
                            {
                                Agent agent = agentGroup.FindAgent(agentID);
                                if (agent != null)
                                {
                                    //Определение пути агента
                                    int[] wayAgent = agent.FindAgentWay(inputData, statistics);
                                    agentGroup.AddWayAgent(wayAgent, agent.idAgent);

                                    //Отправление пути в очередь на кластер
                                    queueOnCluster.AddToQueue(wayAgent, inputData, agent.idAgent);

                                    countFindWay++;
                                }
                            }

                        }));
                    }

                    // Ожидаем завершения всех задач
                    Task.WaitAll(tasks.ToArray());
                    newAgentsList.Clear();

                }
                Console.WriteLine("Задача FindWayTask завершила работу");
                timer.TimeStatistic_End("findWayTask");
            });

            //Определение путей агентов (Последовательный поиск)
            //Task FindWayTask = Task.Run(() =>
            //{
            //    timer.TimeStatistic_Start("findWayTask");

            //    int countFindWay = 0; //Количество найденных путей
            //    int i = 0;

            //    while (countFindWay < inputData.inputParams.countCombinationsV)
            //    {
            //        i++;
            //        //Создаем агента
            //        string id = agentGroup.AddNewAgent();
            //        Agent agent = agentGroup.FindAgent(id);

            //        //Определение пути агента
            //        int[] wayAgent = agent.FindAgentWay(inputData, statistics);
            //        agentGroup.AddWayAgent(wayAgent, id);
            //        countFindWay++;

            //        //Отправление пути в очередь на кластер
            //        queueOnCluster.AddToQueue(wayAgent, inputData, id);

            //    }
            //    Console.WriteLine("Задача FindWayTask завершила работу");
            //    timer.TimeStatistic_End("findWayTask");
            //});

            Task SenderTask = Task.Run(() =>
            {
               
                int countFindWay = 0; //Количество найденных путей
                int i = 0;
                Console.WriteLine(i);
                while (countFindWay < inputData.inputParams.countCombinationsV)
                {
                    //Создаем сокет
                    Request_v2 reqCalculate = new Request_v2();
                    //Получение пути из очереди (из очереди удалили)
                    Calculation calculation = queueOnCluster.GetFromQueue();

                    //Пока нет данных для отправки (очередь пуста)                         //Ожидание освобождения сокета для подключения
                    if (calculation != null)
                    {
                        timer.TimeStatistic_Start("senderTask");

                        try
                        {
                            //Отправление данных на кластер
                            reqCalculate.request.AddData(calculation.TypeOf(), calculation.JSON_Data);
                            Sender data = reqCalculate.Post();

                            //Передача результатов расчета агенту
                            double funcValue = Convert.ToDouble(data.Body);
                            Agent agent = agentGroup.FindAgent(calculation.idAgent);
                            if (agent != null) agent.funcValue = funcValue;


                            int length = calculation.sendData.Way_For_Send.Length;//this.sendData.Way_For_Send.Length;
                            string[] valuesParam = new string[length];
                            for (int j = 0; j < length; j++)
                            {
                                valuesParam[j] = calculation.sendData.Way_For_Send[j].SendValue;
                            }

                            ResultValueFunction valFunction = new ResultValueFunction();
                            valFunction.Set(Convert.ToDouble(data.Body), valuesParam);

                            if (valFunction.valueFunction >= MAX)
                            {
                                maxFunction = valFunction;
                                MAX = valFunction.valueFunction;
                            }

                            //Получение статистики о попадании в % от ожидаемого решения
                            statistics.FindOptimalCount(valFunction.valueFunction, (i + 1), agentGroup.Agents.Count());

                            //Изменение феромонов
                            agent.ChangePheromones(inputData);

                            //Занесение результатов прохода агентов в выходной файл
                            string dataToOutput = fileManager.CreateWriteString(i, "max", maxFunction);
                            fileManager.Write(outputFileName, dataToOutput);

                            Console.WriteLine(dataToOutput);

                            //Удаление агента
                            agentGroup.DeleteAgent(agent.idAgent);

                            //Обновление графа
                            inputData.UpdateParams();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e + "Ошибка связи с кластером при отправлении Calculation");
                        }
                        i++;
                        countFindWay++;

                        timer.TimeStatistic_End("senderTask");
                    }
                    else
                    {
                        Console.WriteLine("Очередь пустая");
                        Task.Delay(1000);
                    }
                }
                Console.WriteLine("Задача SenderTask заверши ла работу");
                
            });


            //Ожидаем выполнение всех задач
            await Task.WhenAll(FindWayTask, SenderTask);
            Console.WriteLine("Задачи выполнены");



            //Отправлка сигнала конца на кластер
            Request_v2 reqCommunication2 = new Request_v2();
            statusCommunication.Set("end");
            try
            {
                reqCommunication2.request.AddData(statusCommunication.TypeOf(), JsonSerializer.Serialize(statusCommunication.Status));
                reqCommunication2.Post();
            }
            catch (Exception e) { Console.WriteLine(e + "Ошибка связи с кластером при отправлении statusCommunication = end"); }

            Console.WriteLine("End senttlement");
            statistics.TimeEnd = DateTime.Now; //Определение времени выполнения
            statistics.WorkTimeLaunch();
            statistics.EmunStatI(inputData.iterationCount); //Сбор статистики о среднем числе переборов за итерацию
            //Запись статистики в файл
            fileManager.WriteStatstring(outputStat, statistics, inputData);
            statistics.LaunchesCount++;

            Console.WriteLine("\n\n\n\nОбщее время работы программы");
            Console.WriteLine(statistics.TimeEnd-statistics.TimeStart);

            timer.TimeStatistic_End("all");

            timer.Write();
        }

        public static int SyncSenttlement(string fileDataName, InputData inputData)
        {
            int countFindWay = 0; //Количество найденных путей
            int countAgent = 0; //Количество пройденных агентов

            FileManager fileManager = new FileManager(); //Менеджер файлов для занесения результатов
            string outputFileName = fileManager.CreateOutputFile(inputData, fileDataName); //Создание файла с выходными результатами

            //Объявление сбора статистики
            StatisticsCollection statistics = new StatisticsCollection();
            string outputStat = fileManager.CreateStatisricFile();

            statistics.StartStatistics(); //Сбор статистики
            statistics.ResetStatistics();  //Сброс статистики по запуску    
            statistics.TimeStart = DateTime.Now;  //Определение времени начала расчета


            //Отправление кластеру запрос на подтверждение
            Request reqCommunication = new Request();
            StatusCommunication statusCommunication = new StatusCommunication("start");
            try
            {
                Console.WriteLine("Отправлен запрос на начало сессии");
                reqCommunication.request.AddData(statusCommunication.TypeOf(), JsonSerializer.Serialize(statusCommunication.Status));
                reqCommunication.Post();
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "Ошибка связи с кластером при отправлении statusCommunication = start");
            }


            //Отправление кластеру запрос на подтверждение
            //ClusterInteraction startWork = new ClusterInteraction();
            //startWork.InitCommunication();


            //Проход по всем итерациям
            for (int i = 0; i < inputData.iterationCount - 2; i++)
            {
                //Console.WriteLine(cancelToken);
                if (countFindWay == inputData.inputParams.countCombinationsV)
                {
                    //Console.WriteLine("EXIT-1: Все пути найдены") ;
                    break;//return 0;
                }


                AgentGroup agentGroup = new AgentGroup(); //Создание группы агентов               
                for (int j = 0; j < inputData.antCount; j++)
                { //Прохождение K агентов
                    AgentPassage(i, inputData, countFindWay, countAgent, agentGroup, statistics);
                }


                agentGroup.AddPheromones(inputData); //Занесение феромонов          
                agentGroup.PheromoneEvaporation(inputData);  //Испарение феромонов

                //Занесение результатов прохода агентов в выходной файл
                string dataToOutput = fileManager.CreateWriteString(i, "max", maxFunction);

                Console.WriteLine(dataToOutput);

                fileManager.Write(outputFileName, dataToOutput);

                //Удаление агентов
                agentGroup.Agents.Clear();

                //Сбор статистики по каждой итерации
                statistics.UniqueSolutionCount = inputData.antCount;
                statistics.CollectingStat(i, statistics.UniqueSolutionCount);
            }


            //Отправлка сигнала конца на кластер
            statusCommunication.Set("end");
            try
            {
                reqCommunication.request.AddData(statusCommunication.TypeOf(), JsonSerializer.Serialize(statusCommunication.Status));
                reqCommunication.Post();
            }
            catch (Exception e) { Console.WriteLine(e + "Ошибка связи с кластером при отправлении statusCommunication = end"); }

            Console.WriteLine("End senttlement");
            statistics.TimeEnd = DateTime.Now; //Определение времени выполнения
            statistics.WorkTimeLaunch();
            statistics.EmunStatI(inputData.iterationCount); //Сбор статистики о среднем числе переборов за итерацию
            //Запись статистики в файл
            fileManager.WriteStatstring(outputStat, statistics, inputData);
            statistics.LaunchesCount++;

            return 0;
        }


        private static void AgentPassage(int NumIteration, InputData inputData, int countFindWay, int countAgent, AgentGroup agentGroup, StatisticsCollection statistics)
        {
            //Проверка просмотрены ли все решения:
            if (countFindWay == inputData.inputParams.countCombinationsV) return;

            //Создание нового агента
            countAgent++;
            string id = agentGroup.AddNewAgent();
            Agent agent = agentGroup.FindAgent(id);

            //Определение пути агента
            int[] wayAgent = agent.FindAgentWay(inputData, statistics);
            agentGroup.AddWayAgent(wayAgent, id);
            countFindWay++;


            //Получение значения целевой функции
            Request reqCalculate = new Request();
            Calculation calculation = new Calculation(wayAgent, inputData);
            try
            {
                reqCalculate.request.AddData(calculation.TypeOf(), calculation.JSON_Data);
                //Console.WriteLine(reqCalculate.request.ToString());
                Sender data = reqCalculate.Post();
                //Console.WriteLine(data.Header);
                //Console.WriteLine(data.Body);

                agent.funcValue = Convert.ToDouble(data.Body);

                int length = calculation.sendData.Way_For_Send.Length;//this.sendData.Way_For_Send.Length;
                string[] valuesParam = new string[length];
                for (int i = 0; i < length; i++)
                {
                    valuesParam[i] = calculation.sendData.Way_For_Send[i].SendValue;
                }

                ResultValueFunction valFunction = new ResultValueFunction();
                valFunction.Set(Convert.ToDouble(data.Body), valuesParam);

                if (valFunction.valueFunction >= MAX)
                {
                    maxFunction = valFunction;
                    MAX = valFunction.valueFunction;
                }

                //Получение статистики о попадании в % от ожидаемого решения
                statistics.FindOptimalCount(valFunction.valueFunction, (NumIteration + 1), agentGroup.Agents.Count());

            }
            catch (Exception e)
            {
                Console.WriteLine(e + "Ошибка связи с кластером при отправлении Calculation");
            }



            if (countFindWay == inputData.inputParams.countCombinationsV) return; //Проверка просмотрены ли все решения:

        }
    }
}