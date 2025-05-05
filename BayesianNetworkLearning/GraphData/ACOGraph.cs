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
using System.Diagnostics;

namespace BayesianNetworkLearning.GraphData
{
    class ACOGraph
    {
        ParamsForLearning parameters { get; set; }

        public InputData ACO_graph { get; set; }

        double _start = 0;
        double _end = 1;
        double _step = 0.001;
        double Eps = 0.00001;

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
            this.ACO_graph.antCount = 5;
            this.ACO_graph.iterationCount = 0;

            //Заполняем узлы графа
            int paramValueId = 0;
            for (int i = 0; i < countNodes; i += 1)
            {
                Param newParam = new Param();
                newParam.defParam = new ParamDefinition()
                {
                    numParam = i,
                    typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double,
                    valuesCount = countProbobiliry
                };

                //Заполняем значения параметра от 0 до 1 с шагом 0.001
                newParam.valuesParam = new List<ParamValue>();
                int num = 0;
                for (double j = this._start; j <= (this._end + this.Eps); j += this._step)
                {
                    ParamValue newValue = new ParamValue() { idValue = paramValueId, numValue = num, pheromones = 1, valueParam = Convert.ToString(j) };

                    newParam.valuesParam.Add(newValue);
                    num++;
                    paramValueId++;
                }

                this.ACO_graph.inputParams.Params.Add(newParam);
            }
        }

        public void Print()
        {
            this.ACO_graph.Print();
        }

    }
}
