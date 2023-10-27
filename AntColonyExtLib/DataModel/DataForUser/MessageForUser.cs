using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel.DataForUser
{
    public class MessageForUser
    {
        public long Percentage { get; set; }
        public string Message { get; set; }

        public MessageForUser() {
            Percentage=0;
            Message = "";
        }
    }
}
