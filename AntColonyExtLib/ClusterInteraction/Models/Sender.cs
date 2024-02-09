using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.ClusterInteraction.Models
{
    /// <summary>
    /// Универсальный отправитель данных на кластер
    /// </summary>
    internal class Sender
    {
        /// <summary>
        /// Заголовок запроса
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Тело запроса
        /// </summary>
        public string Body { get; set; }


        public Sender() {
            Header = "";
            Body = "";
        }

        /// <summary>
        /// Добавление параметров для запроса на кластер
        /// </summary>
        /// <param name="header"></param>
        /// <param name="body"></param>
        public void AddData(string header, string body)
        {
            Header = header; 
            Body = body;
        }

        /// <summary>
        /// Функция возвращающая тип класса в виде строки
        /// </summary>
        /// <returns></returns>
        public string TypeOf()
        {
            return "Sender";
        }
    }
}
