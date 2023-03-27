using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyClient.Models
{
    public class ParamElems
    {
        /// <summary>
        /// Id Значения
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id параметра
        /// </summary>
        [Required]
        public int IdParam { get; set; }

        /// <summary>
        /// Значение параметра
        /// </summary>
        [Required (ErrorMessage="Не указано значение параметра")]
        [MaxLength(50), MinLength(1)]
        public string ValueParam { get; set; }
    }
}
