using AntColonyExtLib.ClusterInteraction.Models;
using AntColonyExtLib.ClusterInteraction.Models.Calculation;
using AntColonyExtLib.ClusterInteraction.Processing;
using AntColonyExtLib.DataModel;
using AntColonyExtLib.DataModel.Numerators;
using AntColonyExtLib.DataModel.Statistic;
using AntColonyExtLib.FileManager;
using AntColonyExtLib.Processing;
using DebagExtLib.DataFunctions;
using DebagExtLib.Realizations;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace DebagExtLib
{
    internal class Program
    {
        public StreamlinedData ResultsForUser = new StreamlinedData();

        static ResultValueFunction maxFunction = new ResultValueFunction();
        static double MAX = double.MinValue;

        static TimeStatistic timer;

        static FileManager_v2 fileManager_v2 = new FileManager_v2();

        static string outputFile = "OutPutData";

        static async Task Main()
        {
            ParamsForTesting paramsForTesting = new ParamsForTesting();

            while (paramsForTesting.countMultySender <= paramsForTesting.Max_countMultySender)  //Мультипакетная отправка
            {
                while (paramsForTesting.timeDelay <= paramsForTesting.MAX_timeDelay) //Цикл по задержке таймера
                {
                    while (paramsForTesting.threadAgentCount <= paramsForTesting.MAX_threadAgentCount) //По количеству потоков агентов
                    {
                        //Создаем файл для сохранения статистики по времени
                        if (paramsForTesting.TimeStatisticFile)
                        {
                            timer = new TimeStatistic("TimeStatistic");
                            string title = "\nЗапуск от " + DateTime.Now.ToString() + "\nКоличество агентов: " + paramsForTesting.threadAgentCount + "\nЗадержка: " + paramsForTesting.timeDelay + "мс" + "\nРазер пакета: " + paramsForTesting.countMultySender;
                            timer.Write(title);

                        }

                        int startCount = 0;
                        while (startCount < paramsForTesting.startCount_MAX)
                        {
                            paramsForTesting.iterationWriteTimerStatistic = paramsForTesting.START_WriteTimerStatistic;
                            paramsForTesting.iterationWriteTimerStatistic_finedWay = paramsForTesting.START_WriteTimerStatistic;

                            //Создание тестового объекта данных
                            MultiFunctionData inputData = new MultiFunctionData();
                            //Клонируем исходный граф
                            inputData.testInputData.cloneInputParams = (ParamsList)inputData.testInputData.inputParams.Clone();

                            HashStatistic hashStatistic = new HashStatistic(paramsForTesting.START_WriteTimerStatistic, inputData.testInputData.inputParams.countCombinationsV);

                            //Создание выходного файла с результатами
                            if (paramsForTesting.OutPutDataFile)
                            {
                                fileManager_v2.CreateFile(outputFile, inputData.testInputData, true);
                            }

                            //Запуск расчетного блока
                            switch (paramsForTesting.startName)
                            {
                                case "Async_v1": //Каждый раз новое соединение
                                                 //await AsyncSenttlement_v1(fileName, inputData.testInputData, paramsForTesting);
                                    break;
                                case "Async_v2": //Одно соединение на все отправки
                                    await AsyncSenttlement_v2(inputData.testInputData, paramsForTesting);
                                    break;
                                case "Async_v3": //Одно соединение на все отправки
                                    await AsyncSenttlement_v3(inputData.testInputData, paramsForTesting, hashStatistic);
                                    break;
                                case "Async_v4": //Одно соединение на все отправки
                                    await AsyncSenttlement_v4(inputData.testInputData, paramsForTesting, hashStatistic);
                                    break;
                                case "Async_v5": //Отправление нескольких данных на кластер за раз с очередью типа BlockingCollection + Не пересоздаем агенты, а просто меняем ID
                                    await AsyncSenttlement_v5(inputData.testInputData, paramsForTesting, hashStatistic);
                                    break;
                                case "Async_v6": //!Отправление нескольких данных на кластер за раз с очередью типа QueueOnCluster с пересозданием агентов, берем все что есть из очереди и отправляем динамическим пакетом
                                    await AsyncSenttlement_v6(inputData.testInputData, paramsForTesting, hashStatistic);
                                    break;
                                case "Async_v7": //Отправление нескольких данных на кластер за раз с очередью типа QueueOnCluster без пересоздания агентов агентов
                                    //Async_Multi_QueueOnCluster_WithoutOvergue async_Multi_QueueOnCluster_WithoutOvergue = new Async_Multi_QueueOnCluster_WithoutOvergue(timer, maxFunction, MAX, fileManager_v2, outputFile);
                                    //async_Multi_QueueOnCluster_WithoutOvergue.AsyncSenttlement_v7(inputData.testInputData, paramsForTesting);
                                    await AsyncSenttlement_v7(inputData.testInputData, paramsForTesting, hashStatistic);
                                    break;
                                case "Sync":
                                    SyncSenttlement(inputData.testInputData, paramsForTesting);
                                    break;
                                default:
                                    Console.WriteLine("Не выбрана фугкция для запуска!");
                                    return;
                            }

                            //
                            //SyncSenttlement(fileName, inputData.testInputData);

                            //Очистка пямяти, сброс данных по статистики
                            inputData.testInputData.hashTable.Clear(); //Очистка Hash
                            if (paramsForTesting.TimeStatisticFile) timer.Clear();

                            startCount++;
                        }

                        paramsForTesting.threadAgentCount = paramsForTesting.threadAgentCount * 2;
                    }

                    paramsForTesting.timeDelay = paramsForTesting.timeDelay + 100;
                    paramsForTesting.threadAgentCount = paramsForTesting.START_threadAgentCount;
                }

                paramsForTesting.timeDelay = paramsForTesting.START_timeDelay;
                paramsForTesting.countMultySender = paramsForTesting.countMultySender * 2;
            }
        }

        /// <summary>
        /// Отправление нескольких данных на кластер за раз с очередью типа QueueOnCluster без пересоздания агентов
        /// </summary>
        /// <param name="fileDataName"></param>
        /// <param name="inputData"></param>
        /// <param name="paramsForTesting"></param>
        /// <returns></returns>
        public static async Task AsyncSenttlement_v7(InputData inputData, ParamsForTesting paramsForTesting, HashStatistic hashStatistic)
        {
            //int countFindWay = 0; //Количество найденных путей
            //int countAgent = 0; //Количество пройденных агентов

            //Counters counters = new Counters();

            if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("all");

            //Создаем очередь и группу агентов
            QueueOnCluster queueOnCluster = new QueueOnCluster();
            AgentGroup agentGroup = new AgentGroup();
            //Словарь всех агентов
            AgentDictionary agentDictionary = new AgentDictionary();

            //Определение путей агентов (Параллельный поиск)
            Task FindWayTask = Task.Run(() =>
            {
                if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("findWayTask");

                int countFindWay = 0; //Количество найденных путей
                int i = 0;

                List<string> newAgentsList = new List<string>();
                //Создаем группу агентов
                for (int j = 0; j < paramsForTesting.threadAgentCount; j++)
                {
                    string id = agentGroup.AddNewAgent();
                }

                while (countFindWay < (inputData.inputParams.countCombinationsV))
                {
                    foreach (var item in agentGroup.Agents) newAgentsList.Add(item.idAgent);
                    i++;

                    List<Task> tasks = new List<Task>();
                    foreach (string agentID in newAgentsList)
                    {
                        //Console.WriteLine("agentID = " + agentID);
                        tasks.Add(Task.Factory.StartNew(() =>
                        {
                            //Если просмотрены все пути, то выход из задачи
                            int count = Interlocked.Increment(ref countFindWay);//++;

                            //Console.WriteLine("agentID = " + agentID+ "\t countFindWay = "+ countFindWay);
                            if (countFindWay <= inputData.inputParams.countCombinationsV)
                            {
                                Agent agent = agentGroup.FindAgent(agentID);
                                if (agent != null)
                                {
                                    //Определение пути агента
                                    int[] wayAgent = agent.FindAgentWay(inputData, paramsForTesting.hashTableStatus, hashStatistic, count);
                                    agentGroup.AddWayAgent(wayAgent, agent.idAgent);

                                    //Отправление пути в очередь на кластер
                                    //Calculation_v2 calculation = new Calculation_v2(agent.idAgent, wayAgent, inputData);

                                    queueOnCluster.AddToQueue(wayAgent, inputData, agent.idAgent);

                                    agentDictionary.Add(agent); //Добавляем агента в словарь
                                    agent.UpdateID(); //Обновляем ID Агента

                                    //int countFindWay = counters.Add_FindWay();

                                    //Замеряем текущее время работы программы
                                    if (paramsForTesting.TimeStatisticFile && count == paramsForTesting.iterationWriteTimerStatistic_finedWay)
                                    {
                                        timer.TimeStatistic_Interval(paramsForTesting.iterationWriteTimerStatistic_finedWay, "findWayTask");
                                        paramsForTesting.iterationWriteTimerStatistic_finedWay += paramsForTesting.stepWriteTimerStatistic;
                                    }
                                }
                            }
                        }));
                    }

                    // Ожидаем завершения всех задач
                    Task.WaitAll(tasks.ToArray());
                    newAgentsList.Clear();

                }
                Console.WriteLine("countFindWay" + countFindWay);
                Console.WriteLine("hashTable.Count" + inputData.hashTable.Count);
                Console.WriteLine("Задача FindWayTask завершила работу");

                queueOnCluster.End();

                hashStatistic.Print();

                if (paramsForTesting.TimeStatisticFile)
                {
                    timer.TimeStatistic_End("findWayTask");
                    timer.TimeStatistic_Interval(inputData.inputParams.countCombinationsV, "findWayTask");
                }

                
            });

            Task SenderTask = Task.Run(() =>
            {
                Console.WriteLine("\nСтарт задачи SenderTask");
                int countSendWay = 0; //Количество найденных путей
                int i = 0;

                //Создаем сокет
                Request_v2 reqCalculate = new Request_v2();

                //Открываем соединение
                DateTime createSocet_Start = DateTime.Now;
                reqCalculate.Start(paramsForTesting.timeDelay, paramsForTesting.threadAgentCount, paramsForTesting.countMultySender);
                DateTime createSocet_End = DateTime.Now;
                TimeSpan createSocet_time = createSocet_End - createSocet_Start;

                //Создаем экземпляр для множественного отправления данных
                MultyCalculation multyCalculation_req = new MultyCalculation();

                while (!queueOnCluster.IsEnd())
                {
                    //Получение пути из очереди (из очереди удалили)
                    Calculation_v2 calculation = queueOnCluster.GetFromQueue();
                    //int countSendWay = counters.Add_SendWay();

                    //Пока нет данных для отправки (очередь пуста)                         //Ожидание освобождения сокета для подключения
                    if (calculation != null)
                    {
                        if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("senderTask");

                        //Добавляем данные для отправлени
                        if (multyCalculation_req.Length() < paramsForTesting.countMultySender - 1)
                        {
                            multyCalculation_req.Add(calculation);
                        }
                        else //Отправляем набор данных на кластер
                        {
                            multyCalculation_req.Add(calculation);
                            try
                            {
                                //Отправление данных на кластер
                                DateTime request_Start = DateTime.Now;

                                reqCalculate.request.AddData(multyCalculation_req.TypeOf(), multyCalculation_req.GetJSON());
                                Sender data = reqCalculate.Post();

                                data.Print();

                                DateTime request_End = DateTime.Now;
                                TimeSpan request_time = createSocet_End - createSocet_Start;

                                //Получение данных из тела запроса
                                MultyCalculation multyCalculation_res = new MultyCalculation(JsonSerializer.Deserialize<List<Calculation_v2>>(data.Body));

                                foreach (Calculation_v2 item in multyCalculation_res.calculationList)
                                {
                                    //Передача результатов расчета агенту
                                    Agent agent = agentDictionary.Get(item.idAgent);
                                    if (agent != null)
                                    {
                                        agent.funcValue = item.result;

                                        int length = item.Way_For_Send.Length;
                                        string[] valuesParam = new string[length];
                                        for (int j = 0; j < length; j++)
                                        {
                                            valuesParam[j] = item.Way_For_Send[j].SendValue;
                                        }

                                        ResultValueFunction valFunction = new ResultValueFunction();
                                        valFunction.Set(item.result, valuesParam);

                                        if (valFunction.valueFunction >= MAX)
                                        {
                                            maxFunction = valFunction;
                                            MAX = valFunction.valueFunction;
                                        }

                                        if (paramsForTesting.OutPutDataFile) //Записываем результат в файл
                                        {
                                            string dataToOutput = fileManager_v2.CreateWriteString(i, "max", maxFunction);
                                            fileManager_v2.Write(outputFile, dataToOutput);
                                        }

                                        agent.ChangePheromones(inputData); //Изменение феромонов
                                        agentDictionary.Delete(agent.idAgent); //Удаление агента
                                    }

                                }

                                //Обновление графа
                                inputData.UpdateParams();
                                //Очистка запроса
                                multyCalculation_req.Clear();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e + "Ошибка связи с кластером при отправлении Calculation");
                            }
                        }
                        i++;
                        countSendWay++;

                        //countSendWay++; //Подсчет отправленных путей
                        //Замер текущего времени
                        if (paramsForTesting.TimeStatisticFile)
                        {
                            timer.TimeStatistic_End("senderTask");
                            if (countSendWay == paramsForTesting.iterationWriteTimerStatistic)
                            {
                                timer.TimeStatistic_Interval(paramsForTesting.iterationWriteTimerStatistic, "senderTask");
                                paramsForTesting.iterationWriteTimerStatistic += paramsForTesting.stepWriteTimerStatistic;
                            }
                        }
                    }
                    else
                    {
                        Task.Delay(1000);
                    }
                }

                if (multyCalculation_req.Length() != 0)
                {
                    if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("senderTask");

                    try
                    {
                        //Отправление данных на кластер
                        DateTime request_Start = DateTime.Now;

                        reqCalculate.request.AddData(multyCalculation_req.TypeOf(), multyCalculation_req.GetJSON());
                        Sender data = reqCalculate.Post();

                        data.Print();

                        DateTime request_End = DateTime.Now;
                        TimeSpan request_time = createSocet_End - createSocet_Start;

                        //Получение данных из тела запроса
                        MultyCalculation multyCalculation_res = new MultyCalculation(JsonSerializer.Deserialize<List<Calculation_v2>>(data.Body));

                        foreach (Calculation_v2 item in multyCalculation_res.calculationList)
                        {
                            //Передача результатов расчета агенту
                            Agent agent = agentDictionary.Get(item.idAgent);

                            if (agent != null)
                            {
                                agent.funcValue = item.result;

                                int length = item.Way_For_Send.Length;
                                string[] valuesParam = new string[length];
                                for (int j = 0; j < length; j++)
                                {
                                    valuesParam[j] = item.Way_For_Send[j].SendValue;
                                }

                                ResultValueFunction valFunction = new ResultValueFunction();
                                valFunction.Set(item.result, valuesParam);

                                if (valFunction.valueFunction >= MAX)
                                {
                                    maxFunction = valFunction;
                                    MAX = valFunction.valueFunction;
                                }

                                if (paramsForTesting.OutPutDataFile) //Записываем результат в файл
                                {
                                    string dataToOutput = fileManager_v2.CreateWriteString(i, "max", maxFunction);
                                    fileManager_v2.Write(outputFile, dataToOutput);
                                }

                                agent.ChangePheromones(inputData); //Изменение феромонов
                                agentDictionary.Delete(agent.idAgent); //Удаление агента
                            }
                        }

                        //Обновление графа
                        inputData.UpdateParams();
                        //Очистка запроса
                        multyCalculation_req.Clear();
                    }
                    catch (Exception e){Console.WriteLine(e + "Ошибка связи с кластером при отправлении Calculation");}

                    i++;
                    if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_End("senderTask");
                }



                if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Interval(countSendWay, "senderTask");
                
                Console.WriteLine("countSendWay " +countSendWay);
                reqCalculate.End();
                Console.WriteLine("Задача SenderTask завершила работу");
            });


            //Ожидаем выполнение всех задач
            await Task.WhenAll(FindWayTask, SenderTask);
            Console.WriteLine("Задачи выполнены");
            Console.WriteLine("End senttlement");

            if (paramsForTesting.TimeStatisticFile)
            {
                timer.TimeStatistic_End("all");
                timer.Write();
                timer.Write(null, "Timestatistic_Interval");
            }
        }


        /// <summary>
        /// Отправление нескольких данных на кластер за раз с очередью типа QueueOnCluster с пересозданием агентов, берем все что есть из очереди и отправляем динамическим пакетом
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="paramsForTesting"></param>
        /// <returns></returns>
        public static async Task AsyncSenttlement_v6(InputData inputData, ParamsForTesting paramsForTesting, HashStatistic hashStatistic)
        {
            //int countFindWay = 0; //Количество найденных путей
            //int countAgent = 0; //Количество пройденных агентов

            if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("all");

            //Создаем очередь и группу агентов
            QueueOnCluster queueOnCluster = new QueueOnCluster();
            AgentGroup agentGroup = new AgentGroup();
            
            //Определение путей агентов (Параллельный поиск)
            Task FindWayTask = Task.Run(() =>
            {
                if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("findWayTask");

                int countFindWay = 0; //Количество найденных путей
                int i = 0;

                while (countFindWay < inputData.inputParams.countCombinationsV)
                {
                    i++;
                    //Console.WriteLine("task1: countFindWay = " + countFindWay);

                    List<string> newAgentsList = new List<string>();
                    //Создаем группу агентов
                    for (int j = 0; j < paramsForTesting.threadAgentCount; j++)
                    {
                        string id = agentGroup.AddNewAgent();
                        newAgentsList.Add(id);
                    }

                    //Console.WriteLine("i = "+i);
                    List<Task> tasks = new List<Task>();
                    foreach (string agentID in newAgentsList)
                    {
                        //Console.WriteLine("agentID = " + agentID);
                        tasks.Add(Task.Factory.StartNew(() =>
                        {
                            //Если просмотрены все пути, то выход из задачи
                            int count = Interlocked.Increment(ref countFindWay);//++;

                            //Console.WriteLine("agentID = " + agentID+ "\t countFindWay = "+ countFindWay);
                            if (countFindWay <= inputData.inputParams.countCombinationsV)
                            {
                                Agent agent = agentGroup.FindAgent(agentID);
                                if (agent != null)
                                {
                                    //Определение пути агента
                                    int[] wayAgent = agent.FindAgentWay(inputData, paramsForTesting.hashTableStatus);
                                    agentGroup.AddWayAgent(wayAgent, agent.idAgent);

                                    //Отправление пути в очередь на кластер
                                    //Calculation_v2 calculation = new Calculation_v2(agent.idAgent, wayAgent, inputData);

                                    queueOnCluster.AddToQueue(wayAgent, inputData, agent.idAgent);

                                    //countFindWay++;

                                    if (paramsForTesting.TimeStatisticFile && count == paramsForTesting.iterationWriteTimerStatistic_finedWay)
                                    {
                                        timer.TimeStatistic_Interval(paramsForTesting.iterationWriteTimerStatistic_finedWay, "findWayTask");
                                        paramsForTesting.iterationWriteTimerStatistic_finedWay += paramsForTesting.stepWriteTimerStatistic;
                                    }
                                }
                            }

                        }));
                    }

                    // Ожидаем завершения всех задач
                    Task.WaitAll(tasks.ToArray());

                    //Console.WriteLine("-- " + countFindWay);
                    newAgentsList.Clear();

                }
                Console.WriteLine("countFindWay" + countFindWay);
                Console.WriteLine("hashTable.Count" + inputData.hashTable.Count);
                Console.WriteLine("Задача FindWayTask завершила работу");

                queueOnCluster.End();
                hashStatistic.Print();

                if (paramsForTesting.TimeStatisticFile)
                {
                    timer.TimeStatistic_End("findWayTask");
                    timer.TimeStatistic_Interval(inputData.inputParams.countCombinationsV, "findWayTask");
                }
            });

            Task SenderTask = Task.Run(() =>
            {
                Console.WriteLine("\nСтарт задачи SenderTask");
                int countSendWay = 0; //Количество найденных путей
                int i = 0;

                //Создаем сокет
                Request_v2 reqCalculate = new Request_v2();

                //Открываем соединение
                DateTime createSocet_Start = DateTime.Now;
                reqCalculate.Start(paramsForTesting.timeDelay, paramsForTesting.threadAgentCount);
                DateTime createSocet_End = DateTime.Now;
                TimeSpan createSocet_time = createSocet_End - createSocet_Start;
                //Console.WriteLine("Создание сокета: " + createSocet_time.TotalSeconds.ToString().Replace('.', ',')+"\n");

                //Создаем экземпляр для множественного отправления данных
                MultyCalculation multyCalculation_req = new MultyCalculation();

                while (!queueOnCluster.IsEnd())
                {
                    //Получение пути из очереди (из очереди удалили)
                    List<Calculation_v2> calculation = queueOnCluster.GetAll();

                    //Пока нет данных для отправки (очередь пуста)                         //Ожидание освобождения сокета для подключения
                    if (calculation != null)
                    {
                        if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("senderTask");

                        //Добавляем данные для отправлени
                        multyCalculation_req.Add(calculation);

                        try
                        {
                            //Отправление данных на кластер
                            DateTime request_Start = DateTime.Now;

                            reqCalculate.request.AddData(multyCalculation_req.TypeOf(), multyCalculation_req.GetJSON());
                            Sender data = reqCalculate.Post();

                            data.Print();

                            DateTime request_End = DateTime.Now;
                            TimeSpan request_time = createSocet_End - createSocet_Start;

                            //Получение данных из тела запроса
                            MultyCalculation multyCalculation_res = new MultyCalculation(JsonSerializer.Deserialize<List<Calculation_v2>>(data.Body));

                            foreach (Calculation_v2 item in multyCalculation_res.calculationList)
                            {
                                //Передача результатов расчета агенту
                                Agent agent = agentGroup.FindAgent(item.idAgent);
                                if (agent != null)
                                {
                                    agent.funcValue = item.result;


                                    int length = item.Way_For_Send.Length;
                                    string[] valuesParam = new string[length];
                                    for (int j = 0; j < length; j++)
                                    {
                                        valuesParam[j] = item.Way_For_Send[j].SendValue;
                                    }

                                    ResultValueFunction valFunction = new ResultValueFunction();
                                    valFunction.Set(item.result, valuesParam);

                                    if (valFunction.valueFunction >= MAX)
                                    {
                                        maxFunction = valFunction;
                                        MAX = valFunction.valueFunction;
                                    }

                                    //Записываем результат в файл
                                    if (paramsForTesting.OutPutDataFile)
                                    {
                                        string dataToOutput = fileManager_v2.CreateWriteString(i, "max", maxFunction);
                                        fileManager_v2.Write(outputFile, dataToOutput);
                                    }

                                    //Изменение феромонов
                                    agent.ChangePheromones(inputData);

                                    //Удаление агента
                                    agentGroup.DeleteAgent(agent.idAgent);
                                }
                            }

                            //Обновление графа
                            inputData.UpdateParams();
                            //Очистка запроса
                            countSendWay += multyCalculation_res.Length();
                            multyCalculation_req.Clear();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e + "Ошибка связи с кластером при отправлении Calculation");
                        }

                        i++;


                        if (paramsForTesting.TimeStatisticFile)
                        {
                            timer.TimeStatistic_End("senderTask");
                            if (countSendWay == paramsForTesting.iterationWriteTimerStatistic)
                            {
                                timer.TimeStatistic_Interval(paramsForTesting.iterationWriteTimerStatistic, "senderTask");
                                paramsForTesting.iterationWriteTimerStatistic += paramsForTesting.stepWriteTimerStatistic;
                            }
                        }
                    }
                    else
                    {
                        Task.Delay(1000);
                    }
                }

                if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Interval(countSendWay, "senderTask");

                Console.WriteLine("countSendWay " + countSendWay);
                reqCalculate.End();
                Console.WriteLine("Задача SenderTask завершила работу");
            });


            //Ожидаем выполнение всех задач
            await Task.WhenAll(FindWayTask, SenderTask);
            Console.WriteLine("Задачи выполнены");
            Console.WriteLine("End senttlement");

            if (paramsForTesting.TimeStatisticFile)
            {
                timer.TimeStatistic_End("all");
                timer.Write();
                timer.Write(null, "Timestatistic_Interval");
            }
        }

        /// <summary>
        /// Отправление нескольких данных на кластер за раз с очередью типа BlockingCollection + Не пересоздаем агенты, а просто меняем ID
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="paramsForTesting"></param>
        /// <returns></returns>
        public static async Task AsyncSenttlement_v5(InputData inputData, ParamsForTesting paramsForTesting, HashStatistic hashStatistic)
        {
            //int countFindWay = 0; //Количество найденных путей
            //int countAgent = 0; //Количество пройденных агентов

            if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("all");

            //Создаем очередь и группу агентов
            var queue = new BlockingCollection<Calculation_v2>();
            AgentGroup agentGroup = new AgentGroup();
            //Словарь всех агентов
            AgentDictionary agentDictionary = new AgentDictionary();

            //Определение путей агентов (Параллельный поиск)
            Task FindWayTask = Task.Run(() =>
            {
                if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("findWayTask");

                int countFindWay = 0; //Количество найденных путей
                int i = 0;

                List<string> newAgentsList = new List<string>();
                //Создаем группу агентов
                for (int j = 0; j < paramsForTesting.threadAgentCount; j++)
                {
                    string id = agentGroup.AddNewAgent();
                }


                while (countFindWay < inputData.inputParams.countCombinationsV)
                {
                    foreach (var item in agentGroup.Agents)
                    {
                        newAgentsList.Add(item.idAgent);
                    }
                    i++;

                    List<Task> tasks = new List<Task>();
                    foreach (string agentID in newAgentsList)
                    {
                        tasks.Add(Task.Factory.StartNew(() =>
                        {
                            //Если просмотрены все пути, то выход из задачи
                            int count = Interlocked.Increment(ref countFindWay);//++;

                            if (countFindWay <= inputData.inputParams.countCombinationsV)
                            {
                                Agent agent = agentGroup.FindAgent(agentID);
                                if (agent != null)
                                {
                                    //Определение пути агента
                                    int[] wayAgent = agent.FindAgentWay(inputData, paramsForTesting.hashTableStatus);
                                    agentGroup.AddWayAgent(wayAgent, agent.idAgent);

                                    //Отправление пути в очередь на кластер
                                    Calculation_v2 calculation = new Calculation_v2(agent.idAgent, wayAgent, inputData);
                                    queue.Add(calculation);

                                    //Добавляем агента в словарь
                                    agentDictionary.Add(agent);

                                    agent.UpdateID();

                                    //Замеряем текущее время работы программы
                                    if (paramsForTesting.TimeStatisticFile && count == paramsForTesting.iterationWriteTimerStatistic_finedWay)
                                    {
                                        timer.TimeStatistic_Interval(paramsForTesting.iterationWriteTimerStatistic_finedWay, "findWayTask");
                                        paramsForTesting.iterationWriteTimerStatistic_finedWay += paramsForTesting.stepWriteTimerStatistic;
                                    }
                                }
                            }

                        }));
                    }

                    // Ожидаем завершения всех задач
                    Task.WaitAll(tasks.ToArray());
                    newAgentsList.Clear();

                }
                Console.WriteLine("countFindWay" + countFindWay);
                Console.WriteLine("hashTable.Count" + inputData.hashTable.Count);
                Console.WriteLine("Задача FindWayTask завершила работу");

                if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_End("findWayTask");

                // Сообщаем, что больше элементов в очереди не будет
                queue.CompleteAdding();

                hashStatistic.Print();

                if (paramsForTesting.TimeStatisticFile)
                {
                    timer.TimeStatistic_End("findWayTask");
                    timer.TimeStatistic_Interval(inputData.inputParams.countCombinationsV, "findWayTask");
                }
            });

            Task SenderTask = Task.Run(() =>
            {
                Console.WriteLine("\nСтарт задачи SenderTask");
                int countSendWay = 0;
                int i = 0;

                //Создаем сокет
                Request_v2 reqCalculate = new Request_v2();

                //Открываем соединение
                DateTime createSocet_Start = DateTime.Now;
                reqCalculate.Start(paramsForTesting.timeDelay, paramsForTesting.threadAgentCount);
                DateTime createSocet_End = DateTime.Now;
                TimeSpan createSocet_time = createSocet_End - createSocet_Start;
                //Console.WriteLine("Создание сокета: " + createSocet_time.TotalSeconds.ToString().Replace('.', ',')+"\n");

                //Создаем экземпляр для множественного отправления данных
                MultyCalculation multyCalculation_req = new MultyCalculation();

                foreach (Calculation_v2 calculation in queue.GetConsumingEnumerable())
                {//Получаем данные из очереди
                    countSendWay++;

                    if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("senderTask");

                    //Добавляем данные для отправлени
                    if (multyCalculation_req.Length() < paramsForTesting.countMultySender - 1)
                    {
                        multyCalculation_req.Add(calculation);
                    }
                    else //Отправляем набор данных на кластер
                    {
                        multyCalculation_req.Add(calculation);
                        try
                        {
                            //Отправление данных на кластер
                            DateTime request_Start = DateTime.Now;

                            reqCalculate.request.AddData(multyCalculation_req.TypeOf(), multyCalculation_req.GetJSON());
                            Sender data = reqCalculate.Post();

                            data.Print();

                            DateTime request_End = DateTime.Now;
                            TimeSpan request_time = createSocet_End - createSocet_Start;

                            //Получение данных из тела запроса
                            MultyCalculation multyCalculation_res = new MultyCalculation(JsonSerializer.Deserialize<List<Calculation_v2>>(data.Body));

                            foreach (Calculation_v2 item in multyCalculation_res.calculationList)
                            {
                                //Передача результатов расчета агенту
                                Agent agent = agentDictionary.Get(item.idAgent);
                                if (agent != null)
                                {
                                    agent.funcValue = item.result;


                                    int length = item.Way_For_Send.Length;
                                    string[] valuesParam = new string[length];
                                    for (int j = 0; j < length; j++)
                                    {
                                        valuesParam[j] = item.Way_For_Send[j].SendValue;
                                    }

                                    ResultValueFunction valFunction = new ResultValueFunction();
                                    valFunction.Set(item.result, valuesParam);

                                    if (valFunction.valueFunction >= MAX)
                                    {
                                        maxFunction = valFunction;
                                        MAX = valFunction.valueFunction;
                                    }

                                    //Записываем результат в файл
                                    if (paramsForTesting.OutPutDataFile)
                                    {
                                        string dataToOutput = fileManager_v2.CreateWriteString(i, "max", maxFunction);
                                        fileManager_v2.Write(outputFile, dataToOutput);
                                    }

                                    //Изменение феромонов
                                    agent.ChangePheromones(inputData);

                                    //Удаление агента
                                    agentDictionary.Delete(agent.idAgent);
                                }
                            }

                            //Обновление графа
                            inputData.UpdateParams();
                            //Очистка запроса
                            multyCalculation_req.Clear();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e + "Ошибка связи с кластером при отправлении Calculation");
                        }
                    }

                    i++;
                    //Замер текущего времени
                    if (paramsForTesting.TimeStatisticFile)
                    {
                        timer.TimeStatistic_End("senderTask");
                        if (countSendWay == paramsForTesting.iterationWriteTimerStatistic)
                        {
                            timer.TimeStatistic_Interval(paramsForTesting.iterationWriteTimerStatistic, "senderTask");
                            paramsForTesting.iterationWriteTimerStatistic += paramsForTesting.stepWriteTimerStatistic;
                        }
                    }
                }

                if (multyCalculation_req.Length() != 0)
                {
                    if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("senderTask");

                    try
                    {
                        //Отправление данных на кластер
                        DateTime request_Start = DateTime.Now;

                        reqCalculate.request.AddData(multyCalculation_req.TypeOf(), multyCalculation_req.GetJSON());
                        Sender data = reqCalculate.Post();

                        data.Print();

                        DateTime request_End = DateTime.Now;
                        TimeSpan request_time = createSocet_End - createSocet_Start;

                        //Получение данных из тела запроса
                        MultyCalculation multyCalculation_res = new MultyCalculation(JsonSerializer.Deserialize<List<Calculation_v2>>(data.Body));

                        foreach (Calculation_v2 item in multyCalculation_res.calculationList)
                        {
                            //Передача результатов расчета агенту
                            Agent agent = agentGroup.FindAgent(item.idAgent);
                            if (agent != null)
                            {
                                agent.funcValue = item.result;


                                int length = item.Way_For_Send.Length;
                                string[] valuesParam = new string[length];
                                for (int j = 0; j < length; j++)
                                {
                                    valuesParam[j] = item.Way_For_Send[j].SendValue;
                                }

                                ResultValueFunction valFunction = new ResultValueFunction();
                                valFunction.Set(item.result, valuesParam);

                                if (valFunction.valueFunction >= MAX)
                                {
                                    maxFunction = valFunction;
                                    MAX = valFunction.valueFunction;
                                }

                                //Записываем результат в файл
                                if (paramsForTesting.OutPutDataFile)
                                {
                                    string dataToOutput = fileManager_v2.CreateWriteString(i, "max", maxFunction);
                                    fileManager_v2.Write(outputFile, dataToOutput);
                                }


                                //Изменение феромонов
                                agent.ChangePheromones(inputData);

                                //Удаление агента
                                agentDictionary.Delete(agent.idAgent);
                            }
                        }

                        //Обновление графа
                        inputData.UpdateParams();
                        //Очистка запроса
                        multyCalculation_req.Clear();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e + "Ошибка связи с кластером при отправлении Calculation");
                    }

                    i++;
                    if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_End("senderTask");
                }
                if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Interval(countSendWay, "senderTask");

                Console.WriteLine("countSendWay " + countSendWay);
                reqCalculate.End();
                Console.WriteLine("Задача SenderTask завершила работу");

            });


            //Ожидаем выполнение всех задач
            await Task.WhenAll(FindWayTask, SenderTask);
            Console.WriteLine("Задачи выполнены");

            if (paramsForTesting.TimeStatisticFile)
            {

                timer.TimeStatistic_End("all");
                timer.Write();
                timer.Write(null, "Timestatistic_Interval");
            }

            agentDictionary.Clear();
        }


        /// <summary>
        /// Отправление нескольких данных на кластер за раз с очередью типа QueueOnCluster с пересозданием
        /// </summary>
        /// <param name="fileDataName"></param>
        /// <param name="inputData"></param>
        /// <param name="paramsForTesting"></param>
        /// <returns></returns>
        public static async Task AsyncSenttlement_v4(InputData inputData, ParamsForTesting paramsForTesting, HashStatistic hashStatistic)
        {
            //int countFindWay = 0; //Количество найденных путей
            //int countAgent = 0; //Количество пройденных агентов

            if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("all");

            //Создаем очередь и группу агентов
            QueueOnCluster queueOnCluster = new QueueOnCluster();
            AgentGroup agentGroup = new AgentGroup();

            //Определение путей агентов (Параллельный поиск)
            Task FindWayTask = Task.Run(() =>
            {
                if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("findWayTask");

                int countFindWay = 0; //Количество найденных путей
                int i = 0;

                while (countFindWay < inputData.inputParams.countCombinationsV)
                {
                    i++;
                    //Console.WriteLine("task1: countFindWay = " + countFindWay);

                    List<string> newAgentsList = new List<string>();
                    //Создаем группу агентов
                    for (int j = 0; j < paramsForTesting.threadAgentCount; j++)
                    {
                        string id = agentGroup.AddNewAgent();
                        newAgentsList.Add(id);
                    }

                    //Console.WriteLine("i = "+i);
                    List<Task> tasks = new List<Task>();
                    foreach (string agentID in newAgentsList)
                    {
                        //Console.WriteLine("agentID = " + agentID);
                        tasks.Add(Task.Factory.StartNew(() =>
                        {
                            //Если просмотрены все пути, то выход из задачи
                            int count = Interlocked.Increment(ref countFindWay);//++;

                            //Console.WriteLine("agentID = " + agentID+ "\t countFindWay = "+ countFindWay);
                            if (countFindWay <= inputData.inputParams.countCombinationsV)
                            {
                                Agent agent = agentGroup.FindAgent(agentID);
                                if (agent != null)
                                {
                                    //Определение пути агента
                                    int[] wayAgent = agent.FindAgentWay(inputData, paramsForTesting.hashTableStatus);
                                    agentGroup.AddWayAgent(wayAgent, agent.idAgent);

                                    //Отправление пути в очередь на кластер
                                    //Calculation_v2 calculation = new Calculation_v2(agent.idAgent, wayAgent, inputData);

                                    queueOnCluster.AddToQueue(wayAgent, inputData, agent.idAgent);

                                    //countFindWay++;

                                    //Замеряем текущее время работы программы
                                    if (paramsForTesting.TimeStatisticFile && count == paramsForTesting.iterationWriteTimerStatistic_finedWay)
                                    {
                                        timer.TimeStatistic_Interval(paramsForTesting.iterationWriteTimerStatistic_finedWay, "findWayTask");
                                        paramsForTesting.iterationWriteTimerStatistic_finedWay += paramsForTesting.stepWriteTimerStatistic;
                                    }
                                }
                            }

                        }));
                    }

                    // Ожидаем завершения всех задач
                    Task.WaitAll(tasks.ToArray());

                    //Console.WriteLine("-- " + countFindWay);
                    newAgentsList.Clear();

                }
                Console.WriteLine("countFindWay" + countFindWay);
                Console.WriteLine("hashTable.Count" + inputData.hashTable.Count);
                Console.WriteLine("Задача FindWayTask завершила работу");

                queueOnCluster.End();

                hashStatistic.Print();

                if (paramsForTesting.TimeStatisticFile)
                {
                    timer.TimeStatistic_End("findWayTask");
                    timer.TimeStatistic_Interval(inputData.inputParams.countCombinationsV, "findWayTask");
                }
            });

            Task SenderTask = Task.Run(() =>
            {
                Console.WriteLine("\nСтарт задачи SenderTask");
                int countSendWay = 0; //Количество найденных путей
                int i = 0;

                //Создаем сокет
                Request_v2 reqCalculate = new Request_v2();

                //Открываем соединение
                DateTime createSocet_Start = DateTime.Now;
                reqCalculate.Start(paramsForTesting.timeDelay, paramsForTesting.threadAgentCount);
                DateTime createSocet_End = DateTime.Now;
                TimeSpan createSocet_time = createSocet_End - createSocet_Start;
                //Console.WriteLine("Создание сокета: " + createSocet_time.TotalSeconds.ToString().Replace('.', ',')+"\n");

                //Создаем экземпляр для множественного отправления данных
                MultyCalculation multyCalculation_req = new MultyCalculation();

                while (!queueOnCluster.IsEnd())
                {
                    //Получение пути из очереди (из очереди удалили)

                    Calculation_v2 calculation = queueOnCluster.GetFromQueue();

                    //Пока нет данных для отправки (очередь пуста)                         //Ожидание освобождения сокета для подключения
                    if (calculation != null)
                    {
                        countSendWay++;

                        if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("senderTask");

                        //Добавляем данные для отправлени
                        if (multyCalculation_req.Length() < paramsForTesting.countMultySender - 1)
                        {
                            multyCalculation_req.Add(calculation);
                        }
                        else //Отправляем набор данных на кластер
                        {
                            multyCalculation_req.Add(calculation);
                            try
                            {
                                //Отправление данных на кластер
                                DateTime request_Start = DateTime.Now;

                                reqCalculate.request.AddData(multyCalculation_req.TypeOf(), multyCalculation_req.GetJSON());
                                Sender data = reqCalculate.Post();

                                data.Print();

                                DateTime request_End = DateTime.Now;
                                TimeSpan request_time = createSocet_End - createSocet_Start;
                                //Console.WriteLine("Время отправления одного запроса и получение ответа: " + request_time.TotalSeconds.ToString().Replace('.', ','));

                                //data.Print();

                                //Получение данных из тела запроса
                                MultyCalculation multyCalculation_res = new MultyCalculation(JsonSerializer.Deserialize<List<Calculation_v2>>(data.Body));

                                foreach (Calculation_v2 item in multyCalculation_res.calculationList)
                                {
                                    //Передача результатов расчета агенту
                                    Agent agent = agentGroup.FindAgent(item.idAgent);
                                    if (agent != null)
                                    {
                                        agent.funcValue = item.result;


                                        int length = item.Way_For_Send.Length;
                                        string[] valuesParam = new string[length];
                                        for (int j = 0; j < length; j++)
                                        {
                                            valuesParam[j] = item.Way_For_Send[j].SendValue;
                                        }

                                        ResultValueFunction valFunction = new ResultValueFunction();
                                        valFunction.Set(item.result, valuesParam);

                                        if (valFunction.valueFunction >= MAX)
                                        {
                                            maxFunction = valFunction;
                                            MAX = valFunction.valueFunction;
                                        }

                                        //Записываем результат в файл
                                        if (paramsForTesting.OutPutDataFile)
                                        {
                                            string dataToOutput = fileManager_v2.CreateWriteString(i, "max", maxFunction);
                                            fileManager_v2.Write(outputFile, dataToOutput);
                                        }

                                        //Изменение феромонов
                                        agent.ChangePheromones(inputData);

                                        //Удаление агента
                                        agentGroup.DeleteAgent(agent.idAgent);
                                    }
                                }

                                //Обновление графа
                                inputData.UpdateParams();
                                //Очистка запроса
                                multyCalculation_req.Clear();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e + "Ошибка связи с кластером при отправлении Calculation");
                            }
                        }

                        i++;
                        //Замер текущего времени
                        if (paramsForTesting.TimeStatisticFile)
                        {
                            timer.TimeStatistic_End("senderTask");
                            if (countSendWay == paramsForTesting.iterationWriteTimerStatistic)
                            {
                                timer.TimeStatistic_Interval(paramsForTesting.iterationWriteTimerStatistic, "senderTask");
                                paramsForTesting.iterationWriteTimerStatistic += paramsForTesting.stepWriteTimerStatistic;
                            }
                        }

                        //if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_End("senderTask");
                    }
                    else
                    {
                        Task.Delay(1000);
                    }
                }

                if (multyCalculation_req.Length() != 0)
                {
                    if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("senderTask");

                    try
                    {
                        //Отправление данных на кластер
                        DateTime request_Start = DateTime.Now;

                        reqCalculate.request.AddData(multyCalculation_req.TypeOf(), multyCalculation_req.GetJSON());
                        Sender data = reqCalculate.Post();

                        data.Print();

                        DateTime request_End = DateTime.Now;
                        TimeSpan request_time = createSocet_End - createSocet_Start;

                        //Получение данных из тела запроса
                        MultyCalculation multyCalculation_res = new MultyCalculation(JsonSerializer.Deserialize<List<Calculation_v2>>(data.Body));

                        foreach (Calculation_v2 item in multyCalculation_res.calculationList)
                        {
                            //Передача результатов расчета агенту
                            Agent agent = agentGroup.FindAgent(item.idAgent);
                            if (agent != null) agent.funcValue = item.result;


                            int length = item.Way_For_Send.Length;
                            string[] valuesParam = new string[length];
                            for (int j = 0; j < length; j++)
                            {
                                valuesParam[j] = item.Way_For_Send[j].SendValue;
                            }

                            ResultValueFunction valFunction = new ResultValueFunction();
                            valFunction.Set(item.result, valuesParam);

                            if (valFunction.valueFunction >= MAX)
                            {
                                maxFunction = valFunction;
                                MAX = valFunction.valueFunction;
                            }

                            //Записываем результат в файл
                            if (paramsForTesting.OutPutDataFile)
                            {
                                string dataToOutput = fileManager_v2.CreateWriteString(i, "max", maxFunction);
                                fileManager_v2.Write(outputFile, dataToOutput);
                            }

                            //Изменение феромонов
                            agent.ChangePheromones(inputData);

                            //Удаление агента
                            agentGroup.DeleteAgent(agent.idAgent);
                        }

                        //Обновление графа
                        inputData.UpdateParams();
                        //Очистка запроса
                        multyCalculation_req.Clear();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e + "Ошибка связи с кластером при отправлении Calculation");
                    }

                    i++;
                    if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_End("senderTask");
                }

                if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Interval(countSendWay, "senderTask");

                Console.WriteLine("countSendWay " + countSendWay);
                reqCalculate.End();
                Console.WriteLine("Задача SenderTask завершила работу");
            });


            //Ожидаем выполнение всех задач
            await Task.WhenAll(FindWayTask, SenderTask);
            Console.WriteLine("Задачи выполнены");

            Console.WriteLine("End senttlement");

            if (paramsForTesting.TimeStatisticFile)
            {
                timer.TimeStatistic_End("all");
                timer.Write();
                timer.Write(null, "Timestatistic_Interval");
            }
        }


        /// <summary>
        /// Отправление нескольких данных на кластер за раз с очередью типа BlockingCollection
        /// </summary>
        /// <param name="fileDataName"></param>
        /// <param name="inputData"></param>
        /// <param name="paramsForTesting"></param>
        /// <returns></returns>
        public static async Task AsyncSenttlement_v3(InputData inputData, ParamsForTesting paramsForTesting, HashStatistic hashStatistic)
        {
            //int countFindWay = 0; //Количество найденных путей
            //int countAgent = 0; //Количество пройденных агентов 

            if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("all");

            //Создаем очередь и группу агентов
            var queue = new BlockingCollection<Calculation_v2>();
            //QueueOnCluster queueOnCluster = new QueueOnCluster();
            AgentGroup agentGroup = new AgentGroup();

            //Определение путей агентов (Параллельный поиск)
            Task FindWayTask = Task.Run(() =>
            {
                if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("findWayTask");

                int countFindWay = 0; //Количество найденных путей
                int i = 0;

                while (countFindWay < inputData.inputParams.countCombinationsV)
                {
                    i++;
                    //Console.WriteLine("task1: countFindWay = " + countFindWay);

                    List<string> newAgentsList = new List<string>();
                    //Создаем группу агентов
                    for (int j = 0; j < paramsForTesting.threadAgentCount; j++)
                    {
                        string id = agentGroup.AddNewAgent();
                        newAgentsList.Add(id);
                    }

                    //Console.WriteLine("i = "+i);
                    List<Task> tasks = new List<Task>();
                    foreach (string agentID in newAgentsList)
                    {
                        //Console.WriteLine("agentID = " + agentID);
                        tasks.Add(Task.Factory.StartNew(() =>
                        {
                            //Если просмотрены все пути, то выход из задачи
                            int count = Interlocked.Increment(ref countFindWay);//++;

                            //Console.WriteLine("agentID = " + agentID+ "\t countFindWay = "+ countFindWay);
                            if (countFindWay <= inputData.inputParams.countCombinationsV)
                            {
                                Agent agent = agentGroup.FindAgent(agentID);
                                if (agent != null)
                                {
                                    //Определение пути агента
                                    int[] wayAgent = agent.FindAgentWay(inputData, paramsForTesting.hashTableStatus);
                                    agentGroup.AddWayAgent(wayAgent, agent.idAgent);

                                    //Отправление пути в очередь на кластер
                                    Calculation_v2 calculation = new Calculation_v2(agent.idAgent, wayAgent, inputData);
                                    queue.Add(calculation);
                                    //queueOnCluster.AddToQueue(wayAgent, inputData, agent.idAgent);

                                    //countFindWay++;

                                    //Замеряем текущее время работы программы
                                    if (paramsForTesting.TimeStatisticFile && count == paramsForTesting.iterationWriteTimerStatistic_finedWay)
                                    {
                                        timer.TimeStatistic_Interval(paramsForTesting.iterationWriteTimerStatistic_finedWay, "findWayTask");
                                        paramsForTesting.iterationWriteTimerStatistic_finedWay += paramsForTesting.stepWriteTimerStatistic;
                                    }
                                }
                            }

                        }));
                    }

                    // Ожидаем завершения всех задач
                    Task.WaitAll(tasks.ToArray());
                    newAgentsList.Clear();

                }
                Console.WriteLine("countFindWay" + countFindWay);
                Console.WriteLine("hashTable.Count" + inputData.hashTable.Count);
                Console.WriteLine("Задача FindWayTask завершила работу");

                if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_End("findWayTask");

                // Сообщаем, что больше элементов в очереди не будет
                queue.CompleteAdding();

                hashStatistic.Print();

                if (paramsForTesting.TimeStatisticFile)
                {
                    timer.TimeStatistic_End("findWayTask");
                    timer.TimeStatistic_Interval(inputData.inputParams.countCombinationsV, "findWayTask");
                }
            });

            Task SenderTask = Task.Run(() =>
            {
                Console.WriteLine("\nСтарт задачи SenderTask");
                int countSendWay = 0;
                int i = 0;

                //Создаем сокет
                Request_v2 reqCalculate = new Request_v2();

                //Открываем соединение
                DateTime createSocet_Start = DateTime.Now;
                reqCalculate.Start(paramsForTesting.timeDelay, paramsForTesting.threadAgentCount);
                DateTime createSocet_End = DateTime.Now;
                TimeSpan createSocet_time = createSocet_End - createSocet_Start;
                //Console.WriteLine("Создание сокета: " + createSocet_time.TotalSeconds.ToString().Replace('.', ',')+"\n");

                //Создаем экземпляр для множественного отправления данных
                MultyCalculation multyCalculation_req = new MultyCalculation();

                foreach (Calculation_v2 calculation in queue.GetConsumingEnumerable())
                {//Получаем данные из очереди
                    countSendWay++;

                    if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("senderTask");

                    //Добавляем данные для отправлени
                    if (multyCalculation_req.Length() < paramsForTesting.countMultySender - 1)
                    {
                        multyCalculation_req.Add(calculation);
                    }
                    else //Отправляем набор данных на кластер
                    {
                        multyCalculation_req.Add(calculation);
                        try
                        {
                            //Отправление данных на кластер
                            DateTime request_Start = DateTime.Now;

                            reqCalculate.request.AddData(multyCalculation_req.TypeOf(), multyCalculation_req.GetJSON());
                            Sender data = reqCalculate.Post();

                            data.Print();

                            DateTime request_End = DateTime.Now;
                            TimeSpan request_time = createSocet_End - createSocet_Start;
                            //Console.WriteLine("Время отправления одного запроса и получение ответа: " + request_time.TotalSeconds.ToString().Replace('.', ','));

                            //data.Print();

                            //Получение данных из тела запроса
                            MultyCalculation multyCalculation_res = new MultyCalculation(JsonSerializer.Deserialize<List<Calculation_v2>>(data.Body));

                            foreach (Calculation_v2 item in multyCalculation_res.calculationList)
                            {
                                //Передача результатов расчета агенту
                                Agent agent = agentGroup.FindAgent(item.idAgent);
                                if (agent != null)
                                {
                                    agent.funcValue = item.result;


                                    int length = item.Way_For_Send.Length;
                                    string[] valuesParam = new string[length];
                                    for (int j = 0; j < length; j++)
                                    {
                                        valuesParam[j] = item.Way_For_Send[j].SendValue;
                                    }

                                    ResultValueFunction valFunction = new ResultValueFunction();
                                    valFunction.Set(item.result, valuesParam);

                                    if (valFunction.valueFunction >= MAX)
                                    {
                                        maxFunction = valFunction;
                                        MAX = valFunction.valueFunction;
                                    }

                                    //Записываем результат в файл
                                    if (paramsForTesting.OutPutDataFile)
                                    {
                                        string dataToOutput = fileManager_v2.CreateWriteString(i, "max", maxFunction);
                                        fileManager_v2.Write(outputFile, dataToOutput);
                                    }

                                    //Изменение феромонов
                                    agent.ChangePheromones(inputData);

                                    //Удаление агента
                                    agentGroup.DeleteAgent(agent.idAgent);
                                }
                            }

                            //Обновление графа
                            inputData.UpdateParams();
                            //Очистка запроса
                            multyCalculation_req.Clear();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e + "Ошибка связи с кластером при отправлении Calculation");
                        }
                    }


                    i++;
                    //Замер текущего времени
                    if (paramsForTesting.TimeStatisticFile)
                    {
                        timer.TimeStatistic_End("senderTask");
                        if (countSendWay == paramsForTesting.iterationWriteTimerStatistic)
                        {
                            timer.TimeStatistic_Interval(paramsForTesting.iterationWriteTimerStatistic, "senderTask");
                            paramsForTesting.iterationWriteTimerStatistic += paramsForTesting.stepWriteTimerStatistic;
                        }
                    }
                }

                if (multyCalculation_req.Length() != 0)
                {
                    if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("senderTask");

                    try
                    {
                        //Отправление данных на кластер
                        DateTime request_Start = DateTime.Now;

                        reqCalculate.request.AddData(multyCalculation_req.TypeOf(), multyCalculation_req.GetJSON());
                        Sender data = reqCalculate.Post();

                        data.Print();

                        DateTime request_End = DateTime.Now;
                        TimeSpan request_time = createSocet_End - createSocet_Start;
                        //Console.WriteLine("Время отправления одного запроса и получение ответа: " + request_time.TotalSeconds.ToString().Replace('.', ','));

                        //data.Print();

                        //Получение данных из тела запроса
                        MultyCalculation multyCalculation_res = new MultyCalculation(JsonSerializer.Deserialize<List<Calculation_v2>>(data.Body));

                        foreach (Calculation_v2 item in multyCalculation_res.calculationList)
                        {
                            //Передача результатов расчета агенту
                            Agent agent = agentGroup.FindAgent(item.idAgent);
                            if (agent != null)
                            {
                                agent.funcValue = item.result;


                                int length = item.Way_For_Send.Length;
                                string[] valuesParam = new string[length];
                                for (int j = 0; j < length; j++)
                                {
                                    valuesParam[j] = item.Way_For_Send[j].SendValue;
                                }

                                ResultValueFunction valFunction = new ResultValueFunction();
                                valFunction.Set(item.result, valuesParam);

                                if (valFunction.valueFunction >= MAX)
                                {
                                    maxFunction = valFunction;
                                    MAX = valFunction.valueFunction;
                                }

                                //Записываем результат в файл
                                if (paramsForTesting.OutPutDataFile)
                                {
                                    string dataToOutput = fileManager_v2.CreateWriteString(i, "max", maxFunction);
                                    fileManager_v2.Write(outputFile, dataToOutput);
                                }

                                //Изменение феромонов
                                agent.ChangePheromones(inputData);

                                //Удаление агента
                                agentGroup.DeleteAgent(agent.idAgent);
                            }
                        }

                        //Обновление графа
                        inputData.UpdateParams();
                        //Очистка запроса
                        multyCalculation_req.Clear();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e + "Ошибка связи с кластером при отправлении Calculation");
                    }

                    i++;
                    if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_End("senderTask");
                }
                if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Interval(countSendWay, "senderTask");

                Console.WriteLine("countSendWay " + countSendWay);
                reqCalculate.End();
                Console.WriteLine("Задача SenderTask завершила работу");

            });


            //Ожидаем выполнение всех задач
            await Task.WhenAll(FindWayTask, SenderTask);
            Console.WriteLine("Задачи выполнены");

            Console.WriteLine("End senttlement");

            if (paramsForTesting.TimeStatisticFile)
            {
                timer.TimeStatistic_End("all");
                timer.Write();
                timer.Write(null, "Timestatistic_Interval");
            }
        }


        /// <summary>
        /// Асинхронный метод через BlockingCollection
        /// </summary>
        /// <param name="fileDataName"></param>
        /// <param name="inputData"></param>
        /// <param name="threadAgentCount"></param>
        /// <param name="timeDelay"></param>
        /// <param name="hashTableStatus"></param>
        /// <returns></returns>
        public static async Task AsyncSenttlement_v2(InputData inputData, ParamsForTesting paramsForTesting)
        {
            int countFindWay = 0; //Количество найденных путей
            int countAgent = 0; //Количество пройденных агентов

            if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("all");

            //Создаем очередь и группу агентов
            var queue = new BlockingCollection<Calculation>();
            //QueueOnCluster queueOnCluster = new QueueOnCluster();
            AgentGroup agentGroup = new AgentGroup();

            //Определение путей агентов (Параллельный поиск)
            Task FindWayTask = Task.Run(() =>
            {
                if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("findWayTask");

                int countFindWay = 0; //Количество найденных путей
                int i = 0;

                while (countFindWay < inputData.inputParams.countCombinationsV)
                {
                    i++;

                    List<string> newAgentsList = new List<string>();
                    //Создаем группу агентов
                    for (int j = 0; j < paramsForTesting.threadAgentCount; j++)
                    {
                        string id = agentGroup.AddNewAgent();
                        newAgentsList.Add(id);
                    }

                    //Console.WriteLine("i = "+i);
                    List<Task> tasks = new List<Task>();
                    foreach (string agentID in newAgentsList)
                    {
                        //Console.WriteLine("agentID = " + agentID);
                        tasks.Add(Task.Factory.StartNew(() =>
                        {
                            //Если просмотрены все пути, то выход из задачи
                            Interlocked.Increment(ref countFindWay);//++;

                            //Console.WriteLine("agentID = " + agentID+ "\t countFindWay = "+ countFindWay);
                            if (countFindWay <= inputData.inputParams.countCombinationsV)
                            {
                                Agent agent = agentGroup.FindAgent(agentID);
                                if (agent != null)
                                {
                                    //Определение пути агента
                                    int[] wayAgent = agent.FindAgentWay(inputData, paramsForTesting.hashTableStatus);
                                    agentGroup.AddWayAgent(wayAgent, agent.idAgent);

                                    //Отправление пути в очередь на кластер
                                    Calculation calculation = new Calculation(wayAgent, inputData, agent.idAgent);
                                    queue.Add(calculation);
                                }
                            }

                        }));
                    }

                    // Ожидаем завершения всех задач
                    Task.WaitAll(tasks.ToArray());

                    //Console.WriteLine("-- " + countFindWay);
                    newAgentsList.Clear();

                }
                Console.WriteLine("countFindWay" + countFindWay);
                Console.WriteLine("hashTable.Count" + inputData.hashTable.Count);
                Console.WriteLine("Задача FindWayTask завершила работу");

                if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_End("findWayTask");

                // Сообщаем, что больше элементов в очереди не будет
                queue.CompleteAdding();
            });

            Task SenderTask = Task.Run(() =>
            {
                Console.WriteLine("\nСтарт задачи SenderTask");
                int countSendWay = 0;
                int i = 0;

                //Создаем сокет
                Request_v2 reqCalculate = new Request_v2();

                //Открываем соединение
                DateTime createSocet_Start = DateTime.Now;
                reqCalculate.Start(paramsForTesting.timeDelay, paramsForTesting.threadAgentCount);
                DateTime createSocet_End = DateTime.Now;
                TimeSpan createSocet_time = createSocet_End - createSocet_Start;
                //Console.WriteLine("Создание сокета: " + createSocet_time.TotalSeconds.ToString().Replace('.', ',')+"\n");



                foreach (Calculation calculation in queue.GetConsumingEnumerable())
                {//Получаем данные из очереди
                    countSendWay++;

                    if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_Start("senderTask");

                    try
                    {
                        //Отправление данных на кластер
                        DateTime request_Start = DateTime.Now;

                        reqCalculate.request.AddData(calculation.TypeOf(), calculation.JSON_Data);
                        Sender data = reqCalculate.Post();

                        DateTime request_End = DateTime.Now;
                        TimeSpan request_time = createSocet_End - createSocet_Start;
                        //Console.WriteLine("Время отправления одного запроса и получение ответа: " + request_time.TotalSeconds.ToString().Replace('.', ','));

                        //data.Print();

                        //Передача результатов расчета агенту
                        double funcValue = Convert.ToDouble(data.Body);
                        Agent agent = agentGroup.FindAgent(calculation.idAgent);
                        if (agent != null) agent.funcValue = funcValue;


                        int length = calculation.sendData.Way_For_Send.Length;
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

                        //Записываем результат в файл
                        if (paramsForTesting.OutPutDataFile)
                        {
                            string dataToOutput = fileManager_v2.CreateWriteString(i, "max", maxFunction);
                            fileManager_v2.Write(outputFile, dataToOutput);
                        }

                        //Изменение феромонов
                        agent.ChangePheromones(inputData);

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
                    if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_End("senderTask");
                }

                Console.WriteLine("countSendWay" + countSendWay);
                reqCalculate.End();
                Console.WriteLine("Задача SenderTask завершила работу");

            });


            //Ожидаем выполнение всех задач
            await Task.WhenAll(FindWayTask, SenderTask);
            Console.WriteLine("Задачи выполнены");
            Console.WriteLine("End senttlement");

            if (paramsForTesting.TimeStatisticFile)
            {
                timer.TimeStatistic_End("all");

                timer.Write();
            }
        }


        /// <summary>
        /// Асинхронный метод с очередью через List
        /// </summary>
        /// <param name="fileDataName"></param>
        /// <param name="inputData"></param>
        /// <param name="threadAgentCount"></param>
        /// <param name="timeDelay"></param>
        /// <param name="hashTableStatus"></param>
        /// <returns></returns>
        //public static async Task AsyncSenttlement_v1(string fileDataName, InputData inputData, ParamsForTesting paramsForTesting)
        //{
        //    int countFindWay = 0; //Количество найденных путей
        //    int countAgent = 0; //Количество пройденных агентов

        //    FileManager fileManager = new FileManager(); //Менеджер файлов для занесения результатов
        //    string outputFileName = fileManager.CreateOutputFile(inputData, fileDataName); //Создание файла с выходными результатами

        //    //Объявление сбора статистики
        //    StatisticsCollection statistics = new StatisticsCollection();
        //    string outputStat = fileManager.CreateStatisricFile();

        //    statistics.StartStatistics(); //Сбор статистики
        //    statistics.ResetStatistics();  //Сброс статистики по запуску    
        //    statistics.TimeStart = DateTime.Now;  //Определение времени начала расчета

        //    TimeStatistic timer = new TimeStatistic("TimeStatistic");
        //    timer.TimeStatistic_Start("all");

        //    string title = "\nЗапуск от "+ DateTime.Now.ToString() + "\nКоличество агентов: " + paramsForTesting.threadAgentCount + "\nЗадержка: "+ paramsForTesting.timeDelay + "мс";
        //    timer.Write(title);

        //    //Отправление кластеру запрос на подтверждение
        //    Request_v2 reqCommunication = new Request_v2();
        //    StatusCommunication statusCommunication = new StatusCommunication("start", paramsForTesting.timeDelay, paramsForTesting.threadAgentCount);
        //    try
        //    {
        //        Console.WriteLine("Отправлен запрос на начало сессии");
        //        reqCommunication.request.AddData(statusCommunication.TypeOf(), JsonSerializer.Serialize(statusCommunication));
        //        reqCommunication.Post();

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e + "Ошибка связи с кластером при отправлении statusCommunication = start");
        //    }

        //    //Создаем очередь и группу агентов
        //    QueueOnCluster queueOnCluster = new QueueOnCluster();
        //    AgentGroup agentGroup = new AgentGroup();

        //    //Определение путей агентов (Параллельный поиск)
        //    Task FindWayTask = Task.Run(() =>
        //    {
        //        timer.TimeStatistic_Start("findWayTask");

        //        int countFindWay = 0; //Количество найденных путей
        //        int i = 0;

        //        while (countFindWay < inputData.inputParams.countCombinationsV)
        //        {
        //            i++;
        //            //Console.WriteLine("task1: countFindWay = " + countFindWay);

        //            List<string> newAgentsList = new List<string>();
        //            //Создаем группу агентов
        //            for (int j = 0; j < paramsForTesting.threadAgentCount; j++)
        //            {
        //                string id = agentGroup.AddNewAgent();
        //                newAgentsList.Add(id);
        //            }

        //            //Console.WriteLine("i = "+i);
        //            List<Task> tasks = new List<Task>();
        //            foreach (string agentID in newAgentsList)
        //            {
        //                //Console.WriteLine("agentID = " + agentID);
        //                tasks.Add(Task.Factory.StartNew(() => {
        //                    //Если просмотрены все пути, то выход из задачи
        //                    Interlocked.Increment(ref countFindWay);//++;

        //                    //Console.WriteLine("agentID = " + agentID+ "\t countFindWay = "+ countFindWay);
        //                    if (countFindWay <= inputData.inputParams.countCombinationsV)
        //                    {
        //                        Agent agent = agentGroup.FindAgent(agentID);
        //                        if (agent != null)
        //                        {
        //                            //Определение пути агента
        //                            int[] wayAgent = agent.FindAgentWay(inputData, statistics, paramsForTesting.hashTableStatus);
        //                            agentGroup.AddWayAgent(wayAgent, agent.idAgent);

        //                            //Отправление пути в очередь на кластер
        //                            queueOnCluster.AddToQueue(wayAgent, inputData, agent.idAgent);

        //                            //countFindWay++;
        //                        }
        //                    }

        //                }));
        //            }

        //            // Ожидаем завершения всех задач
        //            Task.WaitAll(tasks.ToArray());
        //            //Console.WriteLine("-- " + countFindWay);
        //            newAgentsList.Clear();

        //        }
        //        Console.WriteLine("countFindWay" + countFindWay);
        //        Console.WriteLine("hashTable.Count" + inputData.hashTable.Count);
        //        Console.WriteLine("Задача FindWayTask завершила работу");
        //        timer.TimeStatistic_End("findWayTask");
        //    });

        //    Task SenderTask = Task.Run(() =>
        //    {

        //        int countSendWay = 0; //Количество найденных путей
        //        int i = 0;
        //        while (countSendWay < inputData.inputParams.countCombinationsV)
        //        {
        //            //Console.WriteLine("task2: countSendWay = " + countSendWay);
        //            //Создаем сокет
        //            Request_v2 reqCalculate = new Request_v2();
        //            //Получение пути из очереди (из очереди удалили)
        //            Calculation calculation = queueOnCluster.GetFromQueue();

        //            //Пока нет данных для отправки (очередь пуста)                         //Ожидание освобождения сокета для подключения
        //            if (calculation != null)
        //            {
        //                timer.TimeStatistic_Start("senderTask");

        //                try
        //                {
        //                    //Отправление данных на кластер
        //                    reqCalculate.request.AddData(calculation.TypeOf(), calculation.JSON_Data);
        //                    Sender data = reqCalculate.Post();

        //                    //data.Print();

        //                    //Передача результатов расчета агенту
        //                    double funcValue = Convert.ToDouble(data.Body);
        //                    Agent agent = agentGroup.FindAgent(calculation.idAgent);
        //                    if (agent != null) agent.funcValue = funcValue;


        //                    int length = calculation.sendData.Way_For_Send.Length;
        //                    string[] valuesParam = new string[length];
        //                    for (int j = 0; j < length; j++)
        //                    {
        //                        valuesParam[j] = calculation.sendData.Way_For_Send[j].SendValue;
        //                    }

        //                    ResultValueFunction valFunction = new ResultValueFunction();
        //                    valFunction.Set(Convert.ToDouble(data.Body), valuesParam);

        //                    if (valFunction.valueFunction >= MAX)
        //                    {
        //                        maxFunction = valFunction;
        //                        MAX = valFunction.valueFunction;
        //                    }

        //                    //Получение статистики о попадании в % от ожидаемого решения
        //                    statistics.FindOptimalCount(valFunction.valueFunction, (i + 1), agentGroup.Agents.Count());

        //                    //Изменение феромонов
        //                    agent.ChangePheromones(inputData);

        //                    //Занесение результатов прохода агентов в выходной файл
        //                    string dataToOutput = fileManager.CreateWriteString(i, "max", maxFunction);
        //                    fileManager.Write(outputFileName, dataToOutput);

        //                    //Console.WriteLine(dataToOutput);

        //                    //Удаление агента
        //                    agentGroup.DeleteAgent(agent.idAgent);

        //                    //Обновление графа
        //                    inputData.UpdateParams();
        //                }
        //                catch (Exception e)
        //                {
        //                    Console.WriteLine(e + "Ошибка связи с кластером при отправлении Calculation");
        //                }
        //                i++;
        //                countSendWay++;

        //                timer.TimeStatistic_End("senderTask");
        //            }
        //            else
        //            {
        //                //Console.WriteLine("Очередь пустая");
        //                Task.Delay(1000);
        //            }
        //        }

        //        Console.WriteLine("countSendWay" + countSendWay);
        //        Console.WriteLine("Задача SenderTask завершила работу");

        //    });


        //    //Ожидаем выполнение всех задач
        //    await Task.WhenAll(FindWayTask, SenderTask);
        //    Console.WriteLine("Задачи выполнены");



        //    //Отправлка сигнала конца на кластер
        //    Request_v2 reqCommunication2 = new Request_v2();
        //    statusCommunication.Set("end");
        //    try
        //    {
        //        reqCommunication2.request.AddData(statusCommunication.TypeOf(), JsonSerializer.Serialize(statusCommunication));
        //        reqCommunication2.Post();
        //    }
        //    catch (Exception e) { Console.WriteLine(e + "Ошибка связи с кластером при отправлении statusCommunication = end"); }

        //    Console.WriteLine("End senttlement");
        //    statistics.TimeEnd = DateTime.Now; //Определение времени выполнения
        //    statistics.WorkTimeLaunch();
        //    statistics.EmunStatI(inputData.iterationCount); //Сбор статистики о среднем числе переборов за итерацию
        //    //Запись статистики в файл
        //    fileManager.WriteStatstring(outputStat, statistics, inputData);
        //    statistics.LaunchesCount++;

        //    Console.WriteLine("\nОбщее время работы программы\n\n\n");
        //    Console.WriteLine(statistics.TimeEnd-statistics.TimeStart);

        //    timer.TimeStatistic_End("all");

        //    timer.Write();
        //}


        public static int SyncSenttlement(InputData inputData, ParamsForTesting paramsForTesting)
        {
            string fileDataName = "OutputData";

            int countFindWay = 0; //Количество найденных путей
            int countAgent = 0; //Количество пройденных агентов

            FileManager fileManager = new FileManager(); //Менеджер файлов для занесения результатов
            string outputFileName = fileManager.CreateOutputFile(inputData, fileDataName); //Создание файла с выходными результатами


            //Отправление кластеру запрос на подтверждение
            Request reqCommunication = new Request();
            StatusCommunication statusCommunication = new StatusCommunication("start", paramsForTesting.timeDelay, 0);
            try
            {
                Console.WriteLine("Отправлен запрос на начало сессии");

                reqCommunication.request.AddData(statusCommunication.TypeOf(), JsonSerializer.Serialize(statusCommunication));
                reqCommunication.Post();
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "Ошибка связи с кластером при отправлении statusCommunication = start");
            }

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
                    AgentPassage(i, inputData, countFindWay, countAgent, agentGroup, paramsForTesting.hashTableStatus);
                }


                agentGroup.AddPheromones(inputData); //Занесение феромонов          
                agentGroup.PheromoneEvaporation(inputData);  //Испарение феромонов

                //Занесение результатов прохода агентов в выходной файл
                string dataToOutput = fileManager.CreateWriteString(i, "max", maxFunction);

                Console.WriteLine(dataToOutput);

                fileManager.Write(outputFileName, dataToOutput);

                //Удаление агентов
                agentGroup.Agents.Clear();
            }


            //Отправлка сигнала конца на кластер
            statusCommunication.Set("end");
            try
            {
                reqCommunication.request.AddData(statusCommunication.TypeOf(), JsonSerializer.Serialize(statusCommunication));
                reqCommunication.Post();
            }
            catch (Exception e) { Console.WriteLine(e + "Ошибка связи с кластером при отправлении statusCommunication = end"); }

            Console.WriteLine("End senttlement");
            return 0;
        }

        private static void AgentPassage(int NumIteration, InputData inputData, int countFindWay, int countAgent, AgentGroup agentGroup, bool hashTableStatus)
        {
            //Проверка просмотрены ли все решения:
            if (countFindWay == inputData.inputParams.countCombinationsV) return;

            //Создание нового агента
            countAgent++;
            string id = agentGroup.AddNewAgent();
            Agent agent = agentGroup.FindAgent(id);

            //Определение пути агента
            int[] wayAgent = agent.FindAgentWay(inputData, hashTableStatus);
            agentGroup.AddWayAgent(wayAgent, id);
            countFindWay++;


            //Получение значения целевой функции
            Request reqCalculate = new Request();
            Calculation calculation = new Calculation(wayAgent, inputData);
            try
            {
                reqCalculate.request.AddData(calculation.TypeOf(), calculation.JSON_Data);
                Sender data = reqCalculate.Post();

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

            }
            catch (Exception e)
            {
                Console.WriteLine(e + "Ошибка связи с кластером при отправлении Calculation");
            }

            if (countFindWay == inputData.inputParams.countCombinationsV) return; //Проверка просмотрены ли все решения:

        }
    }
}