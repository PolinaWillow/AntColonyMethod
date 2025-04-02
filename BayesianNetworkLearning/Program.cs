using AntColonyExtLib.ClusterInteraction.Models;
using AntColonyExtLib.ClusterInteraction.Models.Calculation;
using AntColonyExtLib.ClusterInteraction.Processing;
using BayesianNetworkLearning.GraphData;
using System.Text.Json;

namespace BayesianNetworkLearning
{
    public class Program
    {
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

                //Строим граф
                ACOGraph acoGrahp = new ACOGraph(graphInfo);
            }
        }
    }
}
