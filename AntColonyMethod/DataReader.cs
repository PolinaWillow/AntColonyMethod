using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyMethod
{
    class DataReader
    {
        /// <summary>
        /// Адрес файла с данными
        /// </summary>
        public static string PATH_TEST_FILE_DATA = "TestData1.txt"; //Файл тестовых значений

        /// <summary>
        /// Получение исходныхи данных
        /// </summary>
        /// <returns></returns>
        public DataTask GettingInputData()
        {
            DataTask dataTask = new DataTask();
            using (var sr = new StreamReader(PATH_TEST_FILE_DATA))
            {
                //Чтение числа итераций
                string iterationCountStr = sr.ReadLine();
                dataTask.iterationCount = Convert.ToInt32(iterationCountStr);

                //Чтение числа пораметров
                string countParam = sr.ReadLine();
                dataTask.paramCount = Convert.ToInt32(countParam);

                //Определение массива количества значений параметров
                string countValueParam = sr.ReadLine();
                dataTask.valueCount.AddRange(countValueParam.Split(' ').Select(x => Convert.ToInt32(x)).ToList());

                //Считывание всех значений параметров 
                for (int i = 0; i < dataTask.paramCount; i++)
                {
                    string valueParam = sr.ReadLine();
                    dataTask.valueData.AddRange(valueParam.Split(' '));
                }

                sr.Close();
            }

            foreach (int elem in dataTask.valueCount) //Подсчет количества всех путей
            {
                dataTask.controlCount = dataTask.controlCount * elem;
            }

            return dataTask;
        }
    }
}
