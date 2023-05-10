using AntColonyExtLib.DataModel;
using AntColonyExtLib.Processing;
using System.Threading;
using System.Threading.Tasks;

namespace DebagExtLib
{
    internal class Program
    {
        private List<string> results = new List<string>();

        static void Main()
        {
            //Создание объекта данных
            InputData inputData = new InputData();
            inputData.antCount = 5;
            inputData.iterationCount = 6999;
            inputData.inputParams.countCombinationsV = 34992;
            inputData.inputParams.Params = new List<Param>()
            {
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 0, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 4, numValue =0, pheromones =1, valueParam = "1"},
                        new ParamValue(){idValue = 5, numValue =1, pheromones =1, valueParam = "2"},
                        new ParamValue(){idValue = 6, numValue =2, pheromones =1, valueParam = "7"},
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 1, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 7, numValue =0, pheromones =1, valueParam = "1"},
                        new ParamValue(){idValue = 8, numValue =1, pheromones =1, valueParam = "4"},
                        new ParamValue(){idValue = 9, numValue =2, pheromones =1, valueParam = "5"},
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 2, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 2
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 10, numValue =0, pheromones =1, valueParam = "1"},
                        new ParamValue(){idValue = 11, numValue =1, pheromones =1, valueParam = "10"},
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 3, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 12, numValue =0, pheromones =1, valueParam = "1"},
                        new ParamValue(){idValue = 13, numValue =1, pheromones =1, valueParam = "11"},
                        new ParamValue(){idValue = 14, numValue =2, pheromones =1, valueParam = "20"},
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 4, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 2
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 15, numValue =0, pheromones =1, valueParam = "1"},
                        new ParamValue(){idValue = 16, numValue =1, pheromones =1, valueParam = "3"}
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 5, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 17, numValue =0, pheromones =1, valueParam = "5,24"},
                        new ParamValue(){idValue = 18, numValue =1, pheromones =1, valueParam = "5,33"},
                        new ParamValue(){idValue = 19, numValue =2, pheromones =1, valueParam = "5,34"},
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 6, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 20, numValue =0, pheromones =1, valueParam = "15,3"},
                        new ParamValue(){idValue = 21, numValue =1, pheromones =1, valueParam = "15,5"},
                        new ParamValue(){idValue = 22, numValue =2, pheromones =1, valueParam = "16,9"},
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 7, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 2
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 23, numValue =0, pheromones =1, valueParam = "1"},
                        new ParamValue(){idValue = 24, numValue =1, pheromones =1, valueParam = "10"}
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 8, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 25, numValue =0, pheromones =1, valueParam = "0,01"},
                        new ParamValue(){idValue = 26, numValue =1, pheromones =1, valueParam = "0,3"},
                        new ParamValue(){idValue = 27, numValue =2, pheromones =1, valueParam = "0,5"},
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 9, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 28, numValue =0, pheromones =1, valueParam = "100000"},
                        new ParamValue(){idValue = 29, numValue =1, pheromones =1, valueParam = "350000"},
                        new ParamValue(){idValue = 30, numValue =2, pheromones =1, valueParam = "600000"},
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 10, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.String, valuesCount = 2
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 31, numValue =0, pheromones =1, valueParam = "Сильное"},
                        new ParamValue(){idValue = 32, numValue =1, pheromones =1, valueParam = "Слабое"}
                    }
                },
            };
            inputData.Print();

            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = cancelTokenSource.Token;

            SenttlementBlock senttlementBlock = new SenttlementBlock();
            //Task<int> result = senttlementBlock.Senttlement(inputData, cancelToken);

            //Результаты работы программы
            //List<string> results = new List<string>();

            FileManager fileManager = new FileManager();
            string fileName = fileManager.CreateFileName("OutPutFile");

            //Создание задач
            Task[] tasks = new Task[2];
            tasks[0] = new Task(() =>
            {
                senttlementBlock.Senttlement(fileName, inputData, cancelToken);
                Console.WriteLine($"Task{0} finished");
            });
            tasks[1] = new Task(() =>
            {
                while (true)
                {
                    string res = senttlementBlock.ResultsForUser.GetMessage();
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        Console.WriteLine("User Results = " + res);
                        Console.WriteLine();
                        
                        //Передача данных пользователю
                        //await hubContext.Clients.All.SendAsync("Receive", 10, 10);
                    }
                    if ((cancelToken.IsCancellationRequested) || ((tasks[0]).IsCompleted)) {
                        break;
                    }
                    Thread.Sleep(1000);
                }                               
            });

            foreach (var task in tasks) {
                task.Start();
            }
            Console.WriteLine("Завершение метода Main");
            Task.WaitAll(tasks);

            ////Запуск выполняемых задач
            //Task.Run(() => senttlementBlock.Senttlement(inputData, cancelToken));
            //Task.Run(() =>
            //{
            //    string newRes = senttlementBlock.OutputResults();
            //    Console.WriteLine("User Results = " + newRes);
            //}
            //);

        }
    }
}