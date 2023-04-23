using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel
{
    public class ResultValueFunction
    {
        public double valueFunction { get; set; }
        public List<string> valuesParams { get; set; }

        public ResultValueFunction() {
            valueFunction = 0;
            valuesParams = new List<string>();
        }

        public void Set(double value, string[] way) {
            valueFunction = value;
            valuesParams = new List<string>(way);
        }

        public void Print() {
            Console.Write("Max="+this.valueFunction+"; ");
            Console.Write("Way: ");
            for (int i = 0; i < this.valuesParams.Count(); i++) {
                Console.Write(this.valuesParams[i]+", ");
            }
            Console.WriteLine();
        }
    }
}
