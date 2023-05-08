using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel.Statistic
{
    internal class ChangeableParams
    {
        /// <summary>
        /// Количество интервалов попадания
        /// </summary>
        public static int NUM_HIT_PERCENTAGE { get; } = 16;

        /// <summary>
        /// Число запусков программы
        /// </summary>
        public static int NUM_LAUNCHES { get; } = 10;
    }
}
