using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel.Statistic
{
    /// <summary>
    /// Таймер для замера времени
    /// </summary>
    public class Timer
    {
        TimeSpan _allTime { get; set; }
        DateTime _timeStart { get; set; }
        DateTime _timeEnd { get; set; }

        TimeSpan _currentTime { get; set; }
        DateTime _currentStart { get; set; }

        public Timer()
        {
            this._allTime = new TimeSpan();
            this._timeStart = new DateTime();
            this._timeEnd = new DateTime();

            this._currentStart = new DateTime();
            this._currentTime = new TimeSpan();
        }

        /// <summary>
        /// Начать отсчет времени
        /// </summary>
        public void StartTimer()
        {
            this._timeStart = DateTime.Now;
        }

        /// <summary>
        /// Определить конец времени работы и посчитать итоговый результат
        /// </summary>
        /// <returns></returns>
        public TimeSpan EndTimer()
        {
            this._timeEnd = DateTime.Now;
            this._allTime = this._timeEnd - this._timeStart;

            return this._allTime;
        }

        public TimeSpan SumTime()
        {
            this._timeEnd = DateTime.Now;
            this._allTime = this._allTime + ( this._timeEnd - this._timeStart);

            return this._allTime;
        }

        public TimeSpan Current_SumTime()
        {
            this._timeEnd = DateTime.Now;
            this._currentTime = this._currentTime + (this._timeEnd - this._timeStart);

            return this._currentTime;
        }

        public TimeSpan CurrentTime()
        {
            this._timeEnd = DateTime.Now;
            this._currentTime = this._timeEnd - this._timeStart;

            return this._currentTime;
        }

        public DateTime GetStart() { return this._timeStart; }
        public DateTime GetEnd() { return this._timeEnd; }
        public TimeSpan GetCurrent() { return this._currentTime; }
        public TimeSpan Get() { return this._allTime; }
    }
}
