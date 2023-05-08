using AntColonyExtLib.DataModel;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.Processing
{
    internal class FileManager
    {
        /// <summary>
        /// Основное название файла с выходными данными
        /// </summary>
        public static string PATH_TEST_FILE_DATA = "OutputData.txt";

        /// <summary>
        /// Создание имени файла
        /// </summary>
        private string CreateFileName(string nameFile)
        {
            //Получение текущей даты
            DateTime today = DateTime.Now;

            //Форматирование даты
            string createData = Convert.ToString(today);
            createData = createData.Replace(" ", "_");
            createData = createData.Replace(":", "-");
            createData = createData.Replace(".", "-");
            //Формирование имени
            string fileName = "../../../../OutputResultFiles/"+ nameFile + "_" + createData + ".txt";

            return fileName;
        }

        /// <summary>
        /// Сохдание выходного файла и заполнение его информацией о входных данных
        /// </summary>
        /// <param name="inputData">Структура с входными данными</param>
        /// <returns></returns>
        public string CreateOutputFile(InputData inputData) {
            string outputDataFile = CreateFileName("OutputData"); //Получение имени выходного файла

            // Создание файла и запись в него
            FileInfo fileInf = new FileInfo(outputDataFile);

            using (StreamWriter sw = File.CreateText(outputDataFile)/*fileInf.CreateText()*/)
            {
                //Запись набора входных данных               
                sw.WriteLine("Число агентов: "+inputData.antCount);
                sw.WriteLine("Количество итераций: " + inputData.iterationCount);
                sw.WriteLine("Число всех возможных путей: " + inputData.inputParams.countCombinationsV);
                sw.WriteLine("-------------------------------------------------------------------------");
                sw.WriteLine("Входные параметры:");
                foreach (var elem in inputData.inputParams.Params) {
                    sw.Write("№" + elem.defParam.numParam + " Values:");
                    foreach (var val in elem.valuesParam) {
                        sw.Write(" "+val.valueParam+";");
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

        /// <summary>
        /// Запись строки в файл
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="writeString">Записываемая строка</param>
        public void Write(string fileName, string writeString) {
            using (StreamWriter sw = new StreamWriter(fileName, true))
            {
                //Запись результата              
                sw.WriteLine(writeString);
                sw.Close();
            }
        }

        public string CreateWriteString(int iterNum, string typeRes ,ResultValueFunction functionRes) {
            string outputStr = "Итерация №: "+ iterNum+"\n"+typeRes+": "+functionRes.valueFunction+"\nValues:";
            foreach (var elem in functionRes.valuesParams)
            {
                outputStr = outputStr + " " + elem+";";
            }

            return outputStr;
        }
    }
}
