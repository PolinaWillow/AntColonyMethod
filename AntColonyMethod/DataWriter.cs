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
        public string CreateOutputFile(DataTask dataTask) {
            string outputDataFile = CreateFileName(); //Получение имени выходного файла
            Console.WriteLine(outputDataFile);

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
        public void GettingOutputData( string outputDataFile, int numIteration, string[] maxFunction, string[] minFunction) 
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
            }

        }

        /// <summary>
        /// Создание имени файла
        /// </summary>
        public string CreateFileName() {
            //Получение текущей даты
            DateTime today = DateTime.Now;

            //Форматирование даты
            string createData = Convert.ToString(today);
            createData = createData.Replace(" ", "_");
            createData = createData.Replace(":", "-");
            createData = createData.Replace(".", "-");
            //Формирование имени
            string fileName = "ResultsWork/OutputData_" + createData + ".txt";

            return fileName;
        }
    }
}
