using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyMethod
{
    class GrafParams
    {
        public int idParam { get; set; }
        public int numParam { get; set; }
        public TypeNumerator typeParam { get; set; }
        public string valueParam { get; set; }
        public int pheromones { get; set; }

        public int selectNum { get; set; }

        public override string ToString()
        {
            return "IdParam: " + idParam + "   NumParam: " + numParam + "   TypeParam: " + typeParam + "   ValueParam: " + valueParam + "   Pheromones: " + pheromones;
        }
    }
}
