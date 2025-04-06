using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public int startCount_MAX = 50; //Количество запусков
        public int threadAgentCount = 1; //Количество потоков агентов
        public int START_threadAgentCount = 1; //Начальное Количество потоков агентов 
        public int MAX_threadAgentCount = 32; //Максимальное количество потоков агентов

        public int timeDelay = 500; //Задержка
        public int START_timeDelay = 500; //Задержка начальная
        public int MAX_timeDelay = 500; //Максимальная задежка

        public bool hashTableStatus = true; //С хэш таблицей (true) или без нее (false)

        public int stepWriteTimerStatistic = 5000; //Шаг для записи статистики
        public int iterationWriteTimerStatistic = 5000; //Итерации для записи статистики
        public int iterationWriteTimerStatistic_finedWay = 5000;
        public int START_WriteTimerStatistic = 5000; //Начальное значение итераций

        //Параметры для управлением отправки
        public int countMultySender = 1; //Количество одновременно отправляемых данных
        public int Max_countMultySender = 128; //Максимальное количество одновременно отправляемых данных

        //Функция для запуска
        public string startName = "Async_v5";

        //Паремтры для создания или не созданпия файлов лога 
        public bool OutPutDataFile = true; //Выходной файл с результатами решений (Криво соотносится key и value при изменении даты создания файла)
        public bool TimeStatisticFile = true; //Выходной файл с временными характеристиками





    }
}
