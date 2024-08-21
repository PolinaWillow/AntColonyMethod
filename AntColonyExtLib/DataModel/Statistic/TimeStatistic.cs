using AntColonyExtLib.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel.Statistic
{
    public class TimeStatistic
    {
        bool _writeFlag { get; set; }
        FileManager _fileManager { get; set; }
        string _fileName { get; set; }

        /// <summary>
        /// Общее время работы модуля
        /// </summary>
        public Timer all_time { get; set; }

        /// <summary>
        /// Время работы модуля FindWayTask
        /// </summary>
        public Timer findWayTask_time { get; set; }
        /// <summary>
        /// Время работы модуля SenderTask
        /// </summary>
        public Timer senderTask_time { get; set; }

        public TimeStatistic(string fileName = null)
        {
            this.findWayTask_time = new Timer();
            this.senderTask_time = new Timer();
            this.all_time = new Timer();

            //Создание файла для записи
            if (fileName == null) this._writeFlag = false;
            else
            {
                this._fileManager = new FileManager();
                this._fileName = this._fileManager.CreateFileName(fileName);
                this._writeFlag = true;
                if (!File.Exists(this._fileName))
                {
                    _fileManager.CreateTimerFile(this._fileName);
                }
            }
        }

        public void TimeStatistic_Start(string modulNane = null)
        {
            switch (modulNane)
            {
                case "all":
                    this.all_time.StartTimer();
                    break;
                case "findWayTask":
                    this.findWayTask_time.StartTimer();
                    break;
                case "senderTask":
                    this.senderTask_time.StartTimer();
                    break;
                default:
                    this.all_time.StartTimer();
                    this.senderTask_time.StartTimer();
                    this.findWayTask_time.StartTimer();
                    break;
            }
        }

        public void TimeStatistic_End(string modulNane = null)
        {
            switch (modulNane)
            {
                case "all":
                    this.all_time.EndTimer();
                    break;
                case "findWayTask":
                    this.findWayTask_time.EndTimer();
                    break;
                case "senderTask":
                    this.senderTask_time.SumTime();
                    break;
                default:
                    this.all_time.EndTimer();
                    this.senderTask_time.EndTimer();
                    this.findWayTask_time.EndTimer();
                    break;
            }
        }

        public void Write()
        {
            if (this._writeFlag)
            {
                string writeString = this.all_time.Get() + "\t" + this.findWayTask_time.Get() + "\t" + this.senderTask_time.Get();
                _fileManager.Write(this._fileName, writeString);
            }
        }
    }
}
