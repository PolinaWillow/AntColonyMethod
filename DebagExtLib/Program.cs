using AntColonyExtLib;
using AntColonyExtLib.DataModel;
using AntColonyExtLib.Processing;

namespace DebagExtLib
{
    internal class Program
    {
        static void Main()
        {
            //Создание объекта данных
            InputData inputData = DataCreator.CreateInputData();

            int result = SenttlementBlock.Senttlement(inputData);
        }
    }
}