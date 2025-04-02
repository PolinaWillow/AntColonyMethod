using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BayesianNetworkLearning
{
    public class ParamsForLearning
    {
        public double Eps { get; set; } //Точность вероятности 
        public double proprobability_start { get; set; } //Начало интервала определения вероятностей
        public double proprobability_end { get; set; } //Конец интервала определения вероятностей

        public ParamsForLearning(double eps = 0.01, double start = 0, double end = 1)
        {
            this.Eps = eps;
            this.proprobability_start = start;
            this.proprobability_end = end;
        }

        public int GetCount()
        {
            double count = (this.proprobability_end - this.proprobability_start) / this.Eps + 1;
            return Convert.ToInt32(count);
        }
    }
}
