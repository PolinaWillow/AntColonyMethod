using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyMethod
{
    class ChangeableParams
    {
        /// <summary>
        /// Файл исходных тестовых значений
        /// </summary>
        public static string PATH_TEST_FILE_DATA { get; } = "TestData6.txt";

        /// <summary>
        /// Ключ упорядочения графа
        /// FeromonsIncreasing - Упорядочение элементов параметра по возрастанию
        /// ParamsIncreasing - Упорядочение параметров по возрастанию
        /// </summary>
        public static string ORDERING_KEY { get; } = "ParamsIncreasing";

        /// <summary>
        /// Количество попыток генерации нового пути
        /// </summary>
        public static int ATTEMPTS_COUNT { get; } = 10000000;

        /// <summary>
        /// Максимальное количество муравьев
        /// </summary>
        public static int MAX_ANT_COUNT { get; } = 70;

        /// <summary>
        /// Количество интервалов попадания
        /// </summary>
        public static int NUM_HIT_PERCENTAGE { get; } = 16;     

        /// <summary>
        /// Интервал изменения количества муравьев
        /// </summary>
        public static int ANT_INTERVAL { get; } = 5;

        /// <summary>
        /// Начальное количество муравьем
        /// </summary>
        public static int ANT_COUNT { get; } = 5;

        /// <summary>
        /// Число запусков программы
        /// </summary>
        public static int NUM_LAUNCHES { get; } = 10;

        /// <summary>
        /// Общее число феромонов
        /// </summary>
        public static int Q { get; } = 100;

        /// <summary>
        /// Коэфициент пересчета испарения феромонов
        /// </summary>
        public static double L { get; } = 0.9;
    }
}
