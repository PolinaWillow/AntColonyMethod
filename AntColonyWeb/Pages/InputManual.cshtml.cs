using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AntColonyWeb.Pages
{
    public class InputManualModel : PageModel
    {
        public string Message { get; set; }
        /// <summary>
        /// Количество параметров
        /// </summary>
        public int ParamCount { get; set; } = 0;

        public void OnGet()
        {
            //ParamCount = 3;
        }

        /// <summary>
        /// Получение количества параметров из формы и генерация полей ввода параметров
        /// </summary>
        /// <param name="paramCount">Количество параметров</param>
        public void OnPostSaveParamCount(int paramCount)
        {
            if (paramCount <= 0) //Проверка на отрицательные значения
            {
                ParamCount = 0;
            }
            else {
                ParamCount = paramCount;
            }

            //Генерация полей ввода параметров
        }

        /// <summary>
        /// Сохранение значений параметров, генерация файла, отправка на сервер, вывод загрузочной понели расчета, начало расчета
        /// </summary>
        public void OnPostStartWork() {
        }
    }
}
