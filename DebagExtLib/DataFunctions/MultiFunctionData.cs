using AntColonyExtLib.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebagExtLib.DataFunctions
{
    internal class MultiFunctionData
    {
        string _nameFile = "MultiFunction.txt";
        double _start { get; set; }
        double _end { get; set; }
        double _step { get; set; }

        public InputData testInputData { get; set; }

        /// <summary>
        /// Чтение данных из входного файла
        /// </summary>
        void readData()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string _path = Path.Combine(baseDirectory, "DataFunctions", this._nameFile);

            if (File.Exists(_path))
            {
                string[] lines = File.ReadLines(_path).ToArray();

                //Чтение файла
                foreach (string line in lines)
                {
                    string[] lineArr = line.Split("; ");
                    this._start = Convert.ToDouble(lineArr[0]);
                    this._end = Convert.ToDouble(lineArr[1]);
                    this._step = Convert.ToDouble(lineArr[2]);
                }
            }
            else
            {
                Console.WriteLine("Файл отсутствует");
            }
        }

        public MultiFunctionData()
        {
            readData();
            //Console.WriteLine(_start);
            //Console.WriteLine(_end);
            //Console.WriteLine(_step);

            testInputData = new InputData();

            double Eps = 0.00001;
            int count = 0;
            for (double i = this._start;  i <= (this._end + Eps); i += this._step)
            {
                count++;
                //Console.WriteLine(i);
            }
            Console.WriteLine(count);

            //Заполнениие основной информации
            testInputData.inputParams.countCombinationsV = count * count;
            testInputData.antCount = 5;
            testInputData.iterationCount = testInputData.inputParams.countCombinationsV/ testInputData.antCount;

            //Заполнение параметров
            testInputData.inputParams.Params = new List<Param>();

            int paramValueId = 0;
            for (int i = 0; i < 2; i += 1)
            {
                Param newParam = new Param();
                newParam.defParam = new ParamDefinition()
                {
                    numParam = i,
                    typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double,
                    valuesCount = count
                };

                newParam.valuesParam = new List<ParamValue>();

                int num = 0;
                for (double j = this._start; j <= (this._end + Eps); j += this._step)
                {
                    ParamValue newValue = new ParamValue() { idValue = paramValueId, numValue = num, pheromones = 1, valueParam = Convert.ToString(j) };
                    
                    newParam.valuesParam.Add(newValue);
                    num++;
                    paramValueId++;
                }

                testInputData.inputParams.Params.Add(newParam);

            }

            //testInputData.Print();

            //testInputData.inputParams.Params = new List<Param>()
            //{
            //    new Param(){
            //        defParam = new ParamDefinition(){
            //            numParam = 0, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
            //        },
            //        valuesParam = new List<ParamValue>(){
            //            new ParamValue(){idValue = 4, numValue =0, pheromones =1, valueParam = "1"},
            //            new ParamValue(){idValue = 5, numValue =1, pheromones =1, valueParam = "2"},
            //            new ParamValue(){idValue = 6, numValue =2, pheromones =1, valueParam = "7"},
            //        }
            //    },
            //    new Param(){
            //        defParam = new ParamDefinition(){
            //            numParam = 1, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
            //        },
            //        valuesParam = new List<ParamValue>(){
            //            new ParamValue(){idValue = 7, numValue =0, pheromones =1, valueParam = "1"},
            //            new ParamValue(){idValue = 8, numValue =1, pheromones =1, valueParam = "4"},
            //            new ParamValue(){idValue = 9, numValue =2, pheromones =1, valueParam = "5"},
            //        }
            //    },
            //    new Param(){
            //        defParam = new ParamDefinition(){
            //            numParam = 2, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 2
            //        },
            //        valuesParam = new List<ParamValue>(){
            //            new ParamValue(){idValue = 10, numValue =0, pheromones =1, valueParam = "1"},
            //            new ParamValue(){idValue = 11, numValue =1, pheromones =1, valueParam = "10"},
            //        }
            //    },
            //    new Param(){
            //        defParam = new ParamDefinition(){
            //            numParam = 3, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
            //        },
            //        valuesParam = new List<ParamValue>(){
            //            new ParamValue(){idValue = 12, numValue =0, pheromones =1, valueParam = "1"},
            //            new ParamValue(){idValue = 13, numValue =1, pheromones =1, valueParam = "11"},
            //            new ParamValue(){idValue = 14, numValue =2, pheromones =1, valueParam = "20"},
            //        }
            //    },
            //    new Param(){
            //        defParam = new ParamDefinition(){
            //            numParam = 4, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 2
            //        },
            //        valuesParam = new List<ParamValue>(){
            //            new ParamValue(){idValue = 15, numValue =0, pheromones =1, valueParam = "1"},
            //            new ParamValue(){idValue = 16, numValue =1, pheromones =1, valueParam = "3"}
            //        }
            //    },
            //    new Param(){
            //        defParam = new ParamDefinition(){
            //            numParam = 5, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
            //        },
            //        valuesParam = new List<ParamValue>(){
            //            new ParamValue(){idValue = 17, numValue =0, pheromones =1, valueParam = "5,24"},
            //            new ParamValue(){idValue = 18, numValue =1, pheromones =1, valueParam = "5,33"},
            //            new ParamValue(){idValue = 19, numValue =2, pheromones =1, valueParam = "5,34"},
            //        }
            //    },
            //    new Param(){
            //        defParam = new ParamDefinition(){
            //            numParam = 6, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
            //        },
            //        valuesParam = new List<ParamValue>(){
            //            new ParamValue(){idValue = 20, numValue =0, pheromones =1, valueParam = "15,3"},
            //            new ParamValue(){idValue = 21, numValue =1, pheromones =1, valueParam = "15,5"},
            //            new ParamValue(){idValue = 22, numValue =2, pheromones =1, valueParam = "16,9"},
            //        }
            //    },
            //    new Param(){
            //        defParam = new ParamDefinition(){
            //            numParam = 7, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 2
            //        },
            //        valuesParam = new List<ParamValue>(){
            //            new ParamValue(){idValue = 23, numValue =0, pheromones =1, valueParam = "1"},
            //            new ParamValue(){idValue = 24, numValue =1, pheromones =1, valueParam = "10"}
            //        }
            //    },
            //    new Param(){
            //        defParam = new ParamDefinition(){
            //            numParam = 8, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
            //        },
            //        valuesParam = new List<ParamValue>(){
            //            new ParamValue(){idValue = 25, numValue =0, pheromones =1, valueParam = "0,01"},
            //            new ParamValue(){idValue = 26, numValue =1, pheromones =1, valueParam = "0,3"},
            //            new ParamValue(){idValue = 27, numValue =2, pheromones =1, valueParam = "0,5"},
            //        }
            //    },
            //    new Param(){
            //        defParam = new ParamDefinition(){
            //            numParam = 9, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
            //        },
            //        valuesParam = new List<ParamValue>(){
            //            new ParamValue(){idValue = 28, numValue =0, pheromones =1, valueParam = "100000"},
            //            new ParamValue(){idValue = 29, numValue =1, pheromones =1, valueParam = "350000"},
            //            new ParamValue(){idValue = 30, numValue =2, pheromones =1, valueParam = "600000"},
            //        }
            //    },
            //    new Param(){
            //        defParam = new ParamDefinition(){
            //            numParam = 10, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.String, valuesCount = 2
            //        },
            //        valuesParam = new List<ParamValue>(){
            //            new ParamValue(){idValue = 31, numValue =0, pheromones =1, valueParam = "Сильное"},
            //            new ParamValue(){idValue = 32, numValue =1, pheromones =1, valueParam = "Слабое"}
            //        }
            //    },
            //};
        }

        public void Print()
        {
            testInputData.Print();
        }
    }
}
