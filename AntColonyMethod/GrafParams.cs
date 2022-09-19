using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyMethod
{
    class GrafParams
    {
        /// <summary>
        /// Id параметра
        /// </summary>
        public int idParam { get; set; }

        /// <summary>
        /// Номер параметра в случе переупорядочеывания 
        /// </summary>
        public int numParamFact { get; set; }

        /// <summary>
        /// Номер параметра 
        /// </summary>
        public int numParam { get; set; }

        /// <summary>
        /// Тип параметра
        /// </summary>
        public TypeNumerator typeParam { get; set; }
        
        /// <summary>
        /// Номер значения параметра
        /// </summary>
        public int numValueParam { get; set; }

        /// <summary>
        /// Значение параметра
        /// </summary>
        public string valueParam { get; set; }
        
        /// <summary>
        /// Количество феромонов
        /// </summary>
        public double pheromones { get; set; }

        /// <summary>
        /// Номер выбора
        /// </summary>
        public int selectNum { get; set; }

        public override string ToString()
        {
            return "IdParam: " + idParam + "   NumParam: " + numParam + "   NumParamFact: " + numParamFact + "   TypeParam: " + typeParam + "   numValueParam: " + numValueParam + "   ValueParam: " + valueParam + "   Pheromones: " + pheromones;
        }

        /// <summary>
        /// Инициализация графа
        /// </summary>
        /// <returns></returns>
        public int InitialState() {
            pheromones = 1;
            return 0;
        }
    }
}
