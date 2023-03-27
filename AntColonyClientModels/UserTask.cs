using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage = "Не указано имя задачи")]
        [MaxLength(50), MinLength(1)]
        public string Name { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        [Required]
        [MaxLength(50), MinLength(1)]
        public string Create_Data { get; set; }

        /// <summary>
        /// Способ ввода данных
        /// </summary>
        [Required (ErrorMessage = "Не указан способ ввода данных")]
        public Enum_InputMethods? InputMethod { get; set; }
    }
}
