using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.DataModel.Files
{
    public class DataFile
    {
        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Идентификатор доступа
        /// </summary>
        public string ID { get; set; }
        public string GetName { get; set; }
        public DataFile() {
            this.FileName = "";
            this.ID = "";
        }
    }
}
