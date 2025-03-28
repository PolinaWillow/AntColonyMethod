using AntColonyExtLib.FileManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel.Statistic
{
    public class TimeStatistic
    {
        bool _writeFlag { get; set; }
        FileManager_v2 _fileManager { get; set; }
        string _fileName { get; set; }
        string _fileName_current { get; set; }

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

        public IntervalRecords_Dictionary intervalRecords { get; set; }

        /// <summary>
        /// Время поиска по hash-таблице
        /// </summary>
        public Timer findHashTable_time { get; set; }

        public TimeStatistic(string fileName = null)
        {
            this.findWayTask_time = new Timer();
            this.senderTask_time = new Timer();
            this.findHashTable_time = new Timer();
            this.all_time = new Timer();

            this.intervalRecords = new IntervalRecords_Dictionary();

            //Создание файлов для записи
            if (fileName == null) this._writeFlag = false;
            else
            {
                this._fileManager = new FileManager_v2();
                //Создание файла для записи статистики работы конца приложения
                this._fileName = this._fileManager.CreateFileName(fileName);
                this._fileName_current = this._fileManager.CreateFileName(fileName + "_current");

                _fileManager.CreateFile(this._fileName, false);
                _fileManager.CreateFile(this._fileName_current, false);

                this._writeFlag = true;
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
                case "findHashTable":
                    this.findHashTable_time.StartTimer();
                    break;
                default:
                    this.all_time.StartTimer();
                    this.senderTask_time.StartTimer();
                    this.findWayTask_time.StartTimer();
                    break;
            }
        }

        /// <summary>
        /// Текущее воемя работы модуля
        /// </summary>
        /// <param name="modulName"></param>
        public void TimeStatistic_Interval(int interval, string modulName = null)
        {
            switch (modulName)
            {
                case "all":
                    this.all_time.CurrentTime();
                    this.intervalRecords.Add(interval, modulName, this.all_time.GetCurrent());
                    break;
                case "findWayTask":
                    this.findWayTask_time.CurrentTime();
                    this.intervalRecords.Add(interval, modulName, this.findWayTask_time.GetCurrent());
                    break;
                case "senderTask":
                    //this.senderTask_time.Current_SumTime();
                    this.intervalRecords.Add(interval, modulName, this.senderTask_time.Get());
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
                case "findHashTable":
                    this.findHashTable_time.EndTimer();
                    break;
                default:
                    this.all_time.EndTimer();
                    this.senderTask_time.EndTimer();
                    this.findWayTask_time.EndTimer();
                    break;
            }
        }

        public void Clear()
        {
            this.findWayTask_time = new Timer();
            this.senderTask_time = new Timer();
            this.findHashTable_time = new Timer();
            this.all_time = new Timer();
        }

        public void Write(string title = null, string typeTimer = null, string interval = null)
        {
            if (title != null)
            {

                _fileManager.Write(this._fileName_current, title);
                _fileManager.Write(this._fileName, title);

            }
            else if (this._writeFlag)
            {
                string writeString = "";
                switch (typeTimer)
                {
                    case "findHashTable":
                        _fileManager.Write(this._fileName, _fileManager.FormatTime(this.findHashTable_time.Get()));
                        break;
                    case "Timestatistic_Interval": //Записываем весь словарь интервалов
                        writeString = "";
                        foreach (var record in intervalRecords.dictionary)
                        {
                            writeString = record.Value.interval + "\t" + _fileManager.FormatTime(record.Value.current_all_time) + "\t" + _fileManager.FormatTime(record.Value.current_findWayTask_time) + "\t" + _fileManager.FormatTime(record.Value.current_senderTask_time);
                            _fileManager.Write(this._fileName_current, writeString);
                        }

                        break;
                    default:
                        writeString = _fileManager.FormatTime(this.all_time.Get()) + "\t" + _fileManager.FormatTime(this.findWayTask_time.Get()) + "\t" + _fileManager.FormatTime(this.senderTask_time.Get());
                        _fileManager.Write(this._fileName, writeString);
                        break;
                }

            }

        }

        public void Print()
        {
            Console.WriteLine("senderTask_time - " + senderTask_time);
        }
    }
}
