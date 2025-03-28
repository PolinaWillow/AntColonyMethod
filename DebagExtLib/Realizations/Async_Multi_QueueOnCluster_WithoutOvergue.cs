using AntColonyExtLib.ClusterInteraction.Models.Calculation;
using AntColonyExtLib.ClusterInteraction.Models;
using AntColonyExtLib.ClusterInteraction.Processing;
using AntColonyExtLib.DataModel;
using AntColonyExtLib.FileManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AntColonyExtLib.DataModel.Statistic;

namespace DebagExtLib.Realizations
{
    /// <summary>
    /// Асинхронная реализация с мульпакетной отправкой, очередью через QueueOnCluster без пересоздания агентов
    /// </summary>
    public class Async_Multi_QueueOnCluster_WithoutOvergue
    {
        TimeStatistic timer { get; set; }
        ResultValueFunction maxFunction { get; set; }
        double MAX { get; set; }
        FileManager_v2 fileManager_v2 { get; set; }
        string outputFile { get; set; }

        public Async_Multi_QueueOnCluster_WithoutOvergue(TimeStatistic timer, ResultValueFunction maxFunction, double MAX, FileManager_v2 fileManager_v2, string outputFile)
        {
            this.timer = timer;
            this.maxFunction = maxFunction;
            this.MAX = MAX;
            this.fileManager_v2 = fileManager_v2;
            this.outputFile = outputFile;
        }

        public async Task AsyncSenttlement_v7(InputData inputData, ParamsForTesting paramsForTesting)
        {
            int countFindWay = 0; //Количество найденных путей
            int countAgent = 0; //Количество пройденных агентов

            if (paramsForTesting.TimeStatisticFile) this.timer.TimeStatistic_Start("all");

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
                                    queueOnCluster.AddToQueue(wayAgent, inputData, agent.idAgent);

                                    //Добавляем агента в словарь
                                    agentDictionary.Add(agent);
                                    agent.UpdateID();

                                    countFindWay++;

                                    if(paramsForTesting.TimeStatisticFile &&  countFindWay == paramsForTesting.iterationWriteTimerStatistic_finedWay)
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

                if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_End("findWayTask");
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
                                    agentDictionary.Delete(agent.idAgent);
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

                        //Запись временных характеристик по процденным итерациям
                        if (paramsForTesting.TimeStatisticFile && (countSendWay==paramsForTesting.iterationWriteTimerStatistic))
                        {
                            timer.TimeStatistic_Interval(paramsForTesting.iterationWriteTimerStatistic, "senderTask");
                            paramsForTesting.iterationWriteTimerStatistic += paramsForTesting.stepWriteTimerStatistic;
                        }

                        if (paramsForTesting.TimeStatisticFile) timer.TimeStatistic_End("senderTask");
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
                            agentDictionary.Delete(agent.idAgent);
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
    }
}
