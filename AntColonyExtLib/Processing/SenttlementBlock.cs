using AntColonyExtLib.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.Processing
{
    public class SenttlementBlock
    {
        public static async Task<int> Senttlement(InputData inputData, CancellationToken cancelToken)
        {
            Console.WriteLine("Начало расчета");
            int countFindWay=0; //Количество найденных путей
            int countAgent = 0; //Количество пройденных агентов
            double MAX = 0; //Текущий максимум функции
            ResultValueFunction maxFunction = new ResultValueFunction();

            //Проход по всем итерациям
            for (int i = 0; i < inputData.iterationCount; i++) {
                if (countFindWay == inputData.inputParams.countCombinationsV) {
                    Console.WriteLine("EXIT-1: Все пути найдены") ;
                    return 0;
                }
                
                //Создание группы агентов
                AgentGroup agentGroup = new AgentGroup();
                //Прохождение K агентов
                Console.WriteLine ("Текущий максимум: ");
                maxFunction.Print();
                for (int j = 0; j < inputData.antCount; j++) {
                    AgentPassage(inputData, countFindWay, countAgent, agentGroup, MAX, maxFunction);
                }

                //Занесение феромонов
                agentGroup.AddPheromones(inputData);
                //Испарение феромонов
                agentGroup.PheromoneEvaporation(inputData);

                //Занесение результатов прохода агентов в выходной файл

                //Проверка на появление события останова расчетов
                //Проверка токена
                if (cancelToken.IsCancellationRequested) {
                    Console.WriteLine("Отмена выполнения по сигналу от пользователя");
                    return 0;
                }

                //Удаление агентов
                agentGroup.Agents.Clear();
            }
            
            return 0;
        }

        private static void AgentPassage(InputData inputData, int countFindWay,int countAgent, AgentGroup agentGroup, double MAX, ResultValueFunction maxFunction) {
            //Проверка просмотрены ли все решения:
            if (countFindWay == inputData.inputParams.countCombinationsV)
            {
                Console.WriteLine("EXIT-2: Все пути найдены");
                return;
            }
            //Создание нового агента
            countAgent++;
            string id = agentGroup.AddNewAgent();
            Agent agent = agentGroup.FindAgent(id);

            //Определение пути агента
            int[] wayAgent = agent.FindAgentWay(inputData);
            agentGroup.AddWayAgent(wayAgent, id);
            countFindWay++;


            //Получение значения целевой функции и определение текущего найденного максимума
            ClusterInteraction clusterInteractionMax = new ClusterInteraction("val", wayAgent, inputData);
            ResultValueFunction valFunction = clusterInteractionMax.SendWay();
            valFunction.Print();

            if (valFunction.valueFunction >= MAX) {
                maxFunction=valFunction;
                MAX = valFunction.valueFunction;
            }

            //Проверка просмотрены ли все решения:
            if (countFindWay == inputData.inputParams.countCombinationsV)
            {
                Console.WriteLine("EXIT-3: Все пути найдены");
                return;
            }
        }
    }
}
