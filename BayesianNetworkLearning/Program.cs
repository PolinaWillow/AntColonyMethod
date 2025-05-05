using AntColonyExtLib.ClusterInteraction.Models;
using AntColonyExtLib.ClusterInteraction.Models.Calculation;
using AntColonyExtLib.ClusterInteraction.Processing;
using AntColonyExtLib.DataModel;
using AntColonyExtLib.DataModel.Statistic;
using AntColonyExtLib.FileManager;
using BayesianNetworkLearning.GraphData;
using System.Text.Json;

namespace BayesianNetworkLearning
{
    public class Program
    {
        static FileManager_v2 fileManager_v2 = new FileManager_v2();
        static string outputFile = "OutPutData";
        static async Task Main(string[] args)
        {
            ParamsForLearning parameters = new ParamsForLearning(); //Параметры для определения точности обучения

            //СОздаем соединение и отправляем на кластер запрос для получения информации о графе
            Request_v2 reqCalculate = new Request_v2();
            if (reqCalculate.Start()) //Открываем соединение
            {
                Sender ClusterData = reqCalculate.GetData(); //Получаем ответ от кластера
                //ClusterData.Print();

                GraphInfo graphInfo = JsonSerializer.Deserialize<GraphInfo>(ClusterData.Body);
                graphInfo.Print();

                //Строим граф и его клон
                ACOGraph acoGrahp = new ACOGraph(graphInfo);
                //acoGrahp.Print();
                acoGrahp.ACO_graph.cloneInputParams = (ParamsList)acoGrahp.ACO_graph.inputParams.Clone();


                fileManager_v2.CreateFile(outputFile, acoGrahp.ACO_graph, true);

                //Запуск асинхронного ACO
                await AsuncACO(acoGrahp.ACO_graph);
            }
        }

        public static async Task AsuncACO(InputData inputData)
        {

        }
    }
}
