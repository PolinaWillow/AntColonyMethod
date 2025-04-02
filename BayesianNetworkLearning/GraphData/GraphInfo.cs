using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BayesianNetworkLearning.GraphData
{
    public class GraphInfo
    {
        public List<NodeInfo> nodes { get; set; }

        public GraphInfo() { }

        public void Print()
        {
            foreach (var item in this.nodes) item.Print();
        }

        public int GetCountNodes()
        {
            int count = 0;
            foreach (var item in this.nodes) count += item.GetCountCombin();
            return count;
        }
    }
}
