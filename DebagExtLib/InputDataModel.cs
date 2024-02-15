using AntColonyExtLib.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebagExtLib
{
    internal class InputDataModel
    {
        public InputData testInputData { get; set; }

        public InputDataModel()
        {
            testInputData = new InputData();
            testInputData.antCount = 5;
            testInputData.iterationCount = 6999;
            testInputData.inputParams.countCombinationsV = 34992;
            testInputData.inputParams.Params = new List<Param>()
            {
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 0, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 4, numValue =0, pheromones =1, valueParam = "1"},
                        new ParamValue(){idValue = 5, numValue =1, pheromones =1, valueParam = "2"},
                        new ParamValue(){idValue = 6, numValue =2, pheromones =1, valueParam = "7"},
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 1, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 7, numValue =0, pheromones =1, valueParam = "1"},
                        new ParamValue(){idValue = 8, numValue =1, pheromones =1, valueParam = "4"},
                        new ParamValue(){idValue = 9, numValue =2, pheromones =1, valueParam = "5"},
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 2, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 2
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 10, numValue =0, pheromones =1, valueParam = "1"},
                        new ParamValue(){idValue = 11, numValue =1, pheromones =1, valueParam = "10"},
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 3, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 12, numValue =0, pheromones =1, valueParam = "1"},
                        new ParamValue(){idValue = 13, numValue =1, pheromones =1, valueParam = "11"},
                        new ParamValue(){idValue = 14, numValue =2, pheromones =1, valueParam = "20"},
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 4, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 2
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 15, numValue =0, pheromones =1, valueParam = "1"},
                        new ParamValue(){idValue = 16, numValue =1, pheromones =1, valueParam = "3"}
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 5, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 17, numValue =0, pheromones =1, valueParam = "5,24"},
                        new ParamValue(){idValue = 18, numValue =1, pheromones =1, valueParam = "5,33"},
                        new ParamValue(){idValue = 19, numValue =2, pheromones =1, valueParam = "5,34"},
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 6, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 20, numValue =0, pheromones =1, valueParam = "15,3"},
                        new ParamValue(){idValue = 21, numValue =1, pheromones =1, valueParam = "15,5"},
                        new ParamValue(){idValue = 22, numValue =2, pheromones =1, valueParam = "16,9"},
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 7, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 2
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 23, numValue =0, pheromones =1, valueParam = "1"},
                        new ParamValue(){idValue = 24, numValue =1, pheromones =1, valueParam = "10"}
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 8, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 25, numValue =0, pheromones =1, valueParam = "0,01"},
                        new ParamValue(){idValue = 26, numValue =1, pheromones =1, valueParam = "0,3"},
                        new ParamValue(){idValue = 27, numValue =2, pheromones =1, valueParam = "0,5"},
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 9, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.Double, valuesCount = 3
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 28, numValue =0, pheromones =1, valueParam = "100000"},
                        new ParamValue(){idValue = 29, numValue =1, pheromones =1, valueParam = "350000"},
                        new ParamValue(){idValue = 30, numValue =2, pheromones =1, valueParam = "600000"},
                    }
                },
                new Param(){
                    defParam = new ParamDefinition(){
                        numParam = 10, typeParam = AntColonyExtLib.DataModel.Numerators.TypeNumerator.String, valuesCount = 2
                    },
                    valuesParam = new List<ParamValue>(){
                        new ParamValue(){idValue = 31, numValue =0, pheromones =1, valueParam = "Сильное"},
                        new ParamValue(){idValue = 32, numValue =1, pheromones =1, valueParam = "Слабое"}
                    }
                },
            };
        }

        public void Print()
        {
            testInputData.Print();
        }
    }
}
