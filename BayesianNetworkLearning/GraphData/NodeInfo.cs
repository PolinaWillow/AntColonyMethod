using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BayesianNetworkLearning.GraphData
{
    public class NodeInfo
    {
        public string node_id { get; set; }
        public string node_name { get; set; }
        public List<string> combin {get;set;}

        public NodeInfo() { }

        public int GetCountCombin()
        {
            if (this.combin != null) return combin.Count();
            else return 0;
        }

        public void Print()
        {
            string printStr = "node_id: " + this.node_id + "\t node_name: " + this.node_name + "\t combins: [";

            if(this.combin!=null) foreach (var item in this.combin) printStr += item + "; ";
            printStr += "] ";

            Console.WriteLine(printStr);
        }
    }
}
