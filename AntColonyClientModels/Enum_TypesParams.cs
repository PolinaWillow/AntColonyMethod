using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyClient.Models
{
    public enum Enum_TypesParams
    {
        [Description("Числовой")]
        Double = 0,
        [Description("Строковый")]
        String = 1,
        [Description("Bool")]
        Bool = 2,
    }
}
