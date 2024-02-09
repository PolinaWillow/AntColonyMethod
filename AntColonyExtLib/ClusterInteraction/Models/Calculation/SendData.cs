using AntColonyExtLib.DataModel;

namespace AntColonyExtLib.ClusterInteraction.Models.Calculation
{
    public class SendData
    {
        //Отправляемые данные (путь агента)
        public WayForSend[] Way_For_Send { get; set; }

        /// <summary>
        /// Конструктор отправляемых данных
        /// </summary>
        /// <param name="typeSendData">Тип отправляемых значений</param>
        /// <param name="way">Путь агента</param>
        /// <param name="inputData">Граф</param>
        public SendData(int[] way, InputData inputData)
        {

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
        }
    }
}
