using AntColonyExtLib.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.FileManager
{
    public class FilesDictionary
    {
        Dictionary<string, string> _dictionary { get; set; } //Словарь с названиями файлов

        public FilesDictionary()
        {
            _dictionary = new Dictionary<string, string>();
        }

        public bool Add(string key, string value)
        {
            if (this._dictionary.ContainsKey(key) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            {
                return false;
            }

            this._dictionary.Add(key, value);
            return true;
        }

        public string Get(string key)
        {
            if (!this._dictionary.ContainsKey(key)) return null;

            return this._dictionary[key];
        }
    }
}
