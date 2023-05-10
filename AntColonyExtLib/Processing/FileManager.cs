using AntColonyExtLib.DataModel;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntColonyExtLib.DataModel.Statistic;

namespace AntColonyExtLib.Processing
{
    public class FileManager
    {
        /// <summary>
        /// Основное название файла с выходными данными
        /// </summary>
        public static string PATH_TEST_FILE_DATA = "OutputData.txt";

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
            string fileName = /*"../../../../OutputResultFiles/"+*/  nameFile + "_" + createData + ".txt";

            return fileName;
        }

        /// <summary>
        /// Сохдание выходного файла и заполнение его информацией о входных данных
        /// </summary>
        /// <param name="inputData">Структура с входными данными</param>
        /// <returns></returns>
        public string CreateOutputFile(InputData inputData, string outputDataFile)
        {
            //string outputDataFile = CreateFileName("OutputData"); //Получение имени выходного файла

            // Создание файла и запись в него
            FileInfo fileInf = new FileInfo(outputDataFile);

            using (StreamWriter sw = File.CreateText(outputDataFile)/*fileInf.CreateText()*/)
            {
                //Запись набора входных данных               
                sw.WriteLine("Число агентов: " + inputData.antCount);
                sw.WriteLine("Количество итераций: " + inputData.iterationCount);
                sw.WriteLine("Число всех возможных путей: " + inputData.inputParams.countCombinationsV);
                sw.WriteLine("-------------------------------------------------------------------------");
                sw.WriteLine("Входные параметры:");
                foreach (var elem in inputData.inputParams.Params)
                {
                    sw.Write("№" + elem.defParam.numParam + " Values:");
                    foreach (var val in elem.valuesParam)
                    {
                        sw.Write(" " + val.valueParam + ";");
                    }
                    sw.Write(" Type: " + elem.defParam.typeParam.ToString() + "; ValuesCount: " + elem.defParam.valuesCount.ToString());
                    sw.WriteLine();
                }


                sw.WriteLine("-------------------------------------------------------------------------\n\n");
                sw.WriteLine("Результаты работы:");

                sw.Close();
            }

            return outputDataFile;
        }

        public string CreateStatisricFile()
        {
            string outputDataFile = CreateFileName("OutputStatistic"); //Получение имени выходного файла

            // Создание файла и запись в него
            FileInfo fileInf = new FileInfo(outputDataFile);
            using (StreamWriter sw = File.CreateText(outputDataFile)/*fileInf.CreateText()*/)
            {
                sw.WriteLine("Результаты сбора статистики:");
                sw.Close();
            }
            return outputDataFile;
        }

        /// <summary>
        /// Запись строки в файл
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="writeString">Записываемая строка</param>
        public void Write(string fileName, string writeString)
        {
            using (StreamWriter sw = new StreamWriter(fileName, true))
            {
                //Запись результата              
                sw.WriteLine(writeString);
                sw.Close();
            }
        }

        public string CreateWriteString(int iterNum, string typeRes, ResultValueFunction functionRes)
        {
            string outputStr = "Итерация №: " + iterNum + "\n" + typeRes + ": " + functionRes.valueFunction + "\nValues:";
            foreach (var elem in functionRes.valuesParams)
            {
                outputStr = outputStr + " " + elem + ";";
            }

            return outputStr;
        }

        public void WriteStatstring(string outputDataFile, StatisticsCollection statistics, InputData inputData)
        {
            statistics.MIteration = statistics.MIteration / statistics.numLaunches;
            statistics.MSolution = statistics.MSolution / statistics.numLaunches;
            statistics.DIteration = statistics.DIteration / statistics.numLaunches;
            statistics.DSolution = statistics.DSolution / statistics.numLaunches;

            for (int i = 0; i < statistics.NumHitPercentage; i++)
            {
                statistics.МFinctionI[i] = statistics.МFinctionI[i] / statistics.Kol[i];
                statistics.DFinctionI[i] = statistics.DFinctionI[i] / statistics.Kol[i];
                statistics.МFinctionS[i] = statistics.МFinctionS[i] / statistics.Kol[i];
                statistics.DFinctionS[i] = statistics.DFinctionS[i] / statistics.Kol[i];
            }

            using (StreamWriter sw = new StreamWriter(outputDataFile, true))
            {
                //Запись результата              
                sw.WriteLine("Количество агентов: \t" + inputData.antCount + "\tИтерация прогона: " + (statistics.LaunchesCount + 1));
                sw.WriteLine("Число итераций: \t" + inputData.iterationCount);
                sw.WriteLine("Время работы алгоритма: \t" + statistics.WorkTime.ToString() + "\tмс");
                sw.WriteLine("MIteration: \t DIteration: \t MSolution: \t DSolution: \t Среднее количество переборов путей за интервал: \t");
                sw.Write("100%: \t");
                for (int i = 1; i < statistics.NumHitPercentage; i++)
                {
                    sw.Write(statistics.PercentageList[i] * 100 + "% \t");
                }
                sw.Write("\n");
                sw.Write(statistics.LaunchesCount + 1 + "\t" + statistics.MIteration.ToString() + "\t" + statistics.DIteration.ToString() + "\t" + statistics.MSolution.ToString() + "\t" + statistics.DSolution.ToString() + "\t" + statistics.AntEnumI + "\t");


                //Вывод статистики нахождения количества решенияй составляющих какой-либо процент от оптимального решения


                sw.Write(statistics.HitCount[0] + " \t");
                for (int i = 1; i < statistics.NumHitPercentage; i++)
                {
                    sw.Write(statistics.HitCount[i] + " \t");
                }
                sw.Write("\t");

                sw.Write(statistics.МFinctionI[0] + "\t" + statistics.DFinctionI[0] + "\t");
                for (int i = 1; i < statistics.NumHitPercentage; i++)
                {
                    sw.Write(statistics.МFinctionI[i] + " \t" + statistics.DFinctionI[i] + " \t");
                }

                sw.Write("\t" + statistics.МFinctionS[0] + "\t" + statistics.DFinctionS[0] + "\t");
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
