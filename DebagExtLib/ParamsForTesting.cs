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
        //Основные параметры для настройки
        public int startCount_MAX = 1; //Количество запусков
        public int threadAgentCount = 1; //Количество потоков агентов
        public int MAX_threadAgentCount = 1; //Максимальное количество потоков агентов
        public int timeDelay = 500; //Задержка
        public int MAX_timeDelay = 500; //Максимальная задежка
        public bool hashTableStatus = true; //С хэш таблицей (true) или без нее (false) 

        //Параметры для управлением отправки
        //bool isMultySender = true; //Отправление пути на кластер по одному (false) или несколько сразу (true)
        public int countMultySender = 1; //Количество одновременно отправляемых данных

        //Функция для запуска
        public string startName = "Sync";

        //Паремтры для создания или не созданпия файлов лога 
        public bool OutPutDataFile = false; //Выходной файл с результатами решений (Криво соотносится key и value при изменении даты создания файла)
        public bool TimeStatisticFile = true; //Выходной файл с временными характеристиками





    }
}
