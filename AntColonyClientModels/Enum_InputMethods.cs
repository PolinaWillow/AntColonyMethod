using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyClient.Models
{
    public enum Enum_InputMethods
    {
        [Description("Из файла")]
        From_File = 0,
        [Description("Вручную")]
        Manual = 1
    }
}
