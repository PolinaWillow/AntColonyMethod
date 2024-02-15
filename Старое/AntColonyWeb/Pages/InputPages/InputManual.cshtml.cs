using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AntColonyWeb.DataModel;

namespace AntColonyWeb.Pages.InputPages
{
    public class InputManualModel : PageModel
    {
        /// <summary>
        /// Типы параметров
        /// </summary>
        public string[] typesParam = new[] { "Строковый", "Числовой", "Числовой с шагом", "Bool" };
        /// <summary>
        /// Id для заполнения значений каждого типа
        /// </summary>
        public string[] idsTypesValue = new[] { "stringParam", "numParam", "numWithStepParam", "boolParam" };


        /// <summary>
        /// Количество параметров
        /// </summary>
        public int ParamCount { get; set; }

        [BindProperty]
        public ConfigParam newParam { get; set; }

        [BindProperty]
        public List<ConfigParam> Params { get; set; }

        /// <summary>
        /// Отображение Добавленных параметров
        /// </summary>
        public List<ConfigParam> DisplayedParams { get; set; }

        //Добавление нового параметра параметра
        public void OnPost(int numParam)
        {
            newParam.numParam = numParam;
            Params.Add(newParam);
        }

        public IActionResult OnGet()
        {
           return Page();
        }


        public InputManualModel()
        {
            ParamCount = 0;
            newParam = new ConfigParam();
            Params = new() {
                new ConfigParam()
                {
                    numParam= 1, typeParam = "Строковый", valueCountParam = 2, valuesParam= {"Сильное", "Cлабое" }
                },
                new ConfigParam()
                {
                    numParam= 2, typeParam = "Числовой", valueCountParam = 3, valuesParam=  {"122", "1883","32" }
                },
                new ConfigParam()
                {
                    numParam= 3, typeParam = "Bool", valueCountParam = 1, valuesParam= {"True"}
                },
                 new ConfigParam()
                {
                   numParam= 4, typeParam = "Числовой", valueCountParam = 3, valuesParam=  {"122", "1883","32" }
                },
                  new ConfigParam()
                {
                   numParam= 5, typeParam = "Числовой", valueCountParam = 3, valuesParam=  {"122", "1883","32" }
                }
            };
            DisplayedParams = new();
        }
    }
}
