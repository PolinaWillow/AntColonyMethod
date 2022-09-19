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
        public void GettingOutputData(string outputDataFile, StatisticsCollection statistics, DataTask dataTask)
        {
            statistics.MIteration = statistics.MIteration / statistics.numLaunches;
            statistics.MSolution = statistics.MSolution / statistics.numLaunches;
            statistics.DIteration = statistics.DIteration / statistics.numLaunches;
            statistics.DSolution = statistics.DSolution / statistics.numLaunches;

            for (int i = 0; i < statistics.NumHitPercentage; i++) {
                statistics.МFinctionI[i] = statistics.МFinctionI[i] / statistics.Kol[i];
                statistics.DFinctionI[i] = statistics.DFinctionI[i] / statistics.Kol[i];
                statistics.МFinctionS[i] = statistics.МFinctionS[i] / statistics.Kol[i];
                statistics.DFinctionS[i] = statistics.DFinctionS[i] / statistics.Kol[i];
            }

            using (StreamWriter sw = new StreamWriter(outputDataFile, true))
            {
                //Запись результата              
                sw.WriteLine("Количество агентов: \t"+ dataTask.antCount+ "\tИтерация прогона: " + (statistics.LaunchesCount+1));
                sw.WriteLine("Число итераций: \t" + dataTask.iterationCount);
                sw.WriteLine("Время работы алгоритма: \t" + statistics.WorkTime.ToString() + "\tмс");
                sw.WriteLine("MIteration: \t DIteration: \t MSolution: \t DSolution: \t Среднее количество переборов путей за интервал: \t");
                sw.WriteLine(statistics.MIteration.ToString() + "\t" + statistics.DIteration.ToString()+"\t" + statistics.MSolution.ToString()+ "\t" + statistics.DSolution.ToString()+ "\t" + statistics.AntEnumI);                          
                sw.WriteLine("Количество попаданий в интервал: ");

                //Вывод статистики нахождения количества решенияй составляющих какой-либо процент от оптимального решения
                sw.Write("Проценты             : \t100%: \t");
                for (int i = 1; i < statistics.NumHitPercentage; i++)
                {
                    sw.Write((statistics.PercentageList[i] * 100) + "% \t");
                }
                sw.Write("\n");

                sw.Write("Количество попаданий : \t" + statistics.HitCount[0] + "\t");
                for (int i = 1; i < statistics.NumHitPercentage; i++)
                {
                    sw.Write(statistics.HitCount[i] + " \t");
                }
                sw.Write("\n");

                sw.Write("МFinctionI | DFinctionI: \t" + statistics.МFinctionI[0] + "\t" + statistics.DFinctionI[0] + "\t");
                for (int i = 1; i < statistics.NumHitPercentage; i++)
                {
                    sw.Write(statistics.МFinctionI[i] + " \t" + statistics.DFinctionI[i] + " \t");
                }
                sw.Write("\n");              

                sw.Write("MFinctionS | DFinctionI:\t" + statistics.МFinctionS[0] + "\t"+ statistics.DFinctionS[0] + "\t");
                for (int i = 1; i < statistics.NumHitPercentage; i++)
                {
                    sw.Write(statistics.МFinctionS[i] + " \t" + statistics.DFinctionS[i] + " \t");
                }
                sw.Write("\n");
                sw.Write("\n");
                sw.Write("\n");

                sw.Close();
            }

        }        
    }
}
