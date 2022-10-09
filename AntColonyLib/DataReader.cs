using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyLib
{
    public class DataReader
    {
        /// <summary>
        /// Получение исходныхи данных
        /// </summary>
        /// <returns></returns>
        public DataTask GettingInputData()
        {
            DataTask dataTask = new DataTask();
            using (var sr = new StreamReader(ChangeableParams.PATH_TEST_FILE_DATA))
            {
                //Чтение числа итераций
                string iterationCountStr = sr.ReadLine();
                dataTask.iterationCount = Convert.ToInt64(iterationCountStr);

                //Чтение числа пораметров
                string countParam = sr.ReadLine();
                dataTask.graphOriginal.paramCount = Convert.ToInt32(countParam);

                //Определение массива количества значений параметров
                string countValueParam = sr.ReadLine();
                dataTask.graphOriginal.valueCount.AddRange(countValueParam.Split(' ').Select(x => Convert.ToInt32(x)).ToList());

                //Считывание всех значений параметров 
                for (int i = 0; i < dataTask.graphOriginal.paramCount; i++)
                {
                    string valueParam = sr.ReadLine();
                    dataTask.valueData.AddRange(valueParam.Split(' '));
                }

                sr.Close();
            }

            foreach (int elem in dataTask.graphOriginal.valueCount) //Подсчет количества всех путей
            {
                dataTask.controlCount = dataTask.controlCount * elem;
            }

            return dataTask;
        }
    }
}
