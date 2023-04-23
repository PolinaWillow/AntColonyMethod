using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel.ClusterData
{
    public class SendData
    {
        public WayForSend[] Way_For_Send { get; set; }

        public string TypeFunctionValue { get; set; }

        public double CurentMaxMin { get; set; }

        public SendData(string typeFunction, int[] way, InputData inputData, double carentExtr = default(double)) {
            TypeFunctionValue = typeFunction;

            int length = way.Length;
            Way_For_Send = new WayForSend[length];
            for (int i = 0; i < length; i++)
            {
                Way_For_Send[i] = new WayForSend();
                Way_For_Send[i].ValueType = inputData.inputParams.Params[i].defParam.typeParam.ToString();
                //Получение параметра и значения пути
                int velCount = inputData.inputParams.Params[i].valuesParam.Count();
                for (int j = 0; j < velCount; j++)
                {
                    int id = inputData.inputParams.Params[i].valuesParam[j].idValue;
                    if (id == way[i])
                    {
                        Way_For_Send[i].SendValue = inputData.inputParams.Params[i].valuesParam[j].valueParam;
                        break;
                    }
                }
                //Console.WriteLine(Way_For_Send[i].ValueType + " - " + Way_For_Send[i].SendValue + "; ");
            }

            CurentMaxMin = carentExtr;
        }
    }
}
