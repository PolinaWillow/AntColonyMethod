using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyMethod
{
    class DataWriter
    {
        /// <summary>
        /// Файл с выходными данными
        /// </summary>
        public static string PATH_TEST_FILE_DATA = "OutputData.txt";

        /// <summary>
        /// Создание выходного файла и заполнение его входными данными
        /// </summary>
        /// <param name="dataTask">Набор входых данных</param>
        /// <returns></returns>
        public string CreateOutputFile(DataTask dataTask)
        {
            string outputDataFile = CreateFileName("OutputData"); //Получение имени выходного файла

            // Создание файла и запись в него
            FileInfo fileInf = new FileInfo(outputDataFile);

            using (StreamWriter sw = fileInf.CreateText())
            {
                //Запись набора входных данных
                if (dataTask.availabilityThread == false)
                {
                    sw.WriteLine("Работа без разделения на потоки\n");
                }
                else
                {
                    sw.WriteLine("Работа c разделением на потоки\n");
                }
                //Запись графа
                sw.WriteLine("Исходный граф");
                foreach (GrafParams elem in dataTask.graf.Params)
                {
                    sw.WriteLine(Convert.ToString(elem));
                }
                sw.WriteLine("\n\n");

                sw.Close();
            }

            return outputDataFile;
        }

        /// <summary>
        /// Заполнение выходного файла результатами
        /// </summary>
        /// <param name="outputDataFile">Название файла</param>
        /// <param name="numIteration">№ итерации</param>
        /// <param name="maxFunction">Массив зранения максимума</param>
        /// <param name="minFunction">Массив хранения минимума</param>
        public void GettingOutputData(string outputDataFile, int numIteration, string[] maxFunction, string[] minFunction)
        {
            using (StreamWriter sw = new StreamWriter(outputDataFile, true))
            {
                //Запись результата              
                sw.WriteLine("Итерация № " + numIteration);
                string strMinFunction = string.Join(" ", minFunction);
                string strMaxFunction = string.Join(" ", maxFunction);

                sw.WriteLine("Минимум: " + strMinFunction);
                sw.WriteLine("Максимум: " + strMaxFunction);
                sw.WriteLine("\n");

                sw.Close();
            }

        }

        /// <summary>
        /// Создание имени файла
        /// </summary>
        public string CreateFileName(string nameFile)
        {
            //Получение текущей даты
            DateTime today = DateTime.Now;

            //Форматирование даты
            string createData = Convert.ToString(today);
            createData = createData.Replace(" ", "_");
            createData = createData.Replace(":", "-");
            createData = createData.Replace(".", "-");
            //Формирование имени
            string fileName = "ResultsWork/" + nameFile + "_" + createData + ".txt";

            return fileName;
        }

        public string CreateStatisticsOutputFile(StatisticsCollection statisticsCollection)
        {
            string outputDataFile = CreateFileName("Statistics"); //Получение имени выходного файла

            // Создание файла и запись в него
            FileInfo fileInf = new FileInfo(outputDataFile);

            using (StreamWriter sw = fileInf.CreateText())
            {
                sw.WriteLine("Сбор статистики");

                sw.Close();
            }

            return outputDataFile;
        }

        /// <summary>
        /// Заполнение выходного файла со статистикой
        /// </summary>
        /// <param name="outputDataFile">Название выходного файла</param>
        public void GettingOutputData(string outputDataFile, StatisticsCollection statisticsCollection, DataTask dataTask)
        {
            using (StreamWriter sw = new StreamWriter(outputDataFile, true))
            {
                //Запись результата              
                sw.WriteLine("Количество агентов: "+ dataTask.antCount+ "\tИтерация прогона: " + statisticsCollection.LaunchesCount);
                sw.WriteLine("MIteration: " + statisticsCollection.MIteration);
                sw.WriteLine("DIteration: " + statisticsCollection.DIteration);
                sw.WriteLine("MSolution: " + statisticsCollection.MSolution);
                sw.WriteLine("DSolution: " + statisticsCollection.DSolution);
                sw.WriteLine("Количество попаданий в интервал: ");

                sw.WriteLine("100%: " + statisticsCollection.HitCount[0]);
                for (int i=1; i< statisticsCollection.NumHitPercentage; i++) 
                {
                    sw.WriteLine(statisticsCollection.PercentageList[i] + "%: " + statisticsCollection.HitCount[i]);
                }

                sw.WriteLine("\n");

                sw.Close();
            }

        }

    }
}
