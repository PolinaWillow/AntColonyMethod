using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebagExtLib
{
    /// <summary>
    /// Параметры для тестирования и сбора статистики
    /// </summary>
    public class ParamsForTesting
    {
        public int startCount_MAX = 5; //Количество запусков
        public int threadAgentCount = 2; //Количество потоков агентов
        public int MAX_threadAgentCount = 2; //Максимальное количество потоков агентов
        public int timeDelay = 500; //Задержка
        public int MAX_timeDelay = 500; //Максимальная задежка
        public bool hashTableStatus = false; //С хэш таблицей (true) или без нее (false) 
    }
}
