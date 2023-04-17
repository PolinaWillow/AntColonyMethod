using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AntColonyExtLib.DataModel.Numerators;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyClient.Models
{
    public class TaskParams
    {
        /// <summary>
        /// Id параметра
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id задачи
        /// </summary>
        [Required]
        public int IdTask { get; set; }

        /// <summary>
        /// Номер параметра
        /// </summary>
        [Required (ErrorMessage = "Не указано номер параметра")]      
        public int NumParam { get; set; }

        /// <summary>
        /// Тип значений параметра
        /// </summary>
        [Required(ErrorMessage = "Не указан тип значений параметра")]
        public TypeNumerator TypeParam { get; set; }


    }
}
