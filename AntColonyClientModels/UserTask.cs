using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyClient.Models
{
    public class UserTask
    {
        /// <summary>
        /// ID задачи
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название задачи
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Дата создания
        /// </summary>
        public string Create_Data { get; set; }
        /// <summary>
        /// Способ ввода данных
        /// </summary>
        public Enum_InputMethods? InputMethod { get; set; }
    }
}
