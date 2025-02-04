using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.ClusterInteraction.Models.Calculation
{
    public class WayForSend
    {
        public string SendValue { get; set; }

        public string ValueType { get; set; }

        public void Print()
        {
            Console.Write("("+SendValue + ": " + ValueType+")");
        }

    }
}
