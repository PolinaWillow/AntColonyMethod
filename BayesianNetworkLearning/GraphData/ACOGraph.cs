using AntColonyExtLib.DataModel;
using AntColonyExtLib.ClusterInteraction.Models.Calculation;
using AntColonyExtLib.ClusterInteraction.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Numerics;

namespace BayesianNetworkLearning.GraphData
{
    class ACOGraph
    {
        ParamsForLearning parameters { get; set; }

        public InputData ACO_graph { get; set; }

        public ACOGraph(GraphInfo graphInfo)
        {
            this.parameters = new ParamsForLearning(); //Определяем параметры обучения

           
            this.ACO_graph = new InputData();  //Создаем графовою модель

            //Считаем сколько у нас вариантов вероятностей для одного узла, счситаем сколько узлов и возводим в степини
            int countProbobiliry = this.parameters.GetCount();
            Console.WriteLine("Количество возможных значений вероятностей: "+countProbobiliry);
            int countNodes = graphInfo.GetCountNodes();
            Console.WriteLine("Количество узлов: "+ countNodes);

            BigInteger CountWays = BigInteger.Pow((BigInteger)countProbobiliry, countNodes);
            Console.WriteLine("Количество итераций: "+ CountWays);

            this.ACO_graph.inputParams.countCombinationsV = 0; //CountWays;




        }

    }
}
