using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AntColonyWeb.Pages.InputPages
{
    public class InputFileModel : PageModel
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        /// <summary>
        /// Файл с данными
        /// </summary>
        [BindProperty]
        public IFormFile inputFile { get; set; }

        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string filePath { get; set; }
        
        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost() {
            if (inputFile != null)
            {
                filePath = ProcessUploadedFile();
            }          
            return Page();
        }

        /// <summary>
        /// Добавление нового файла на сервер
        /// </summary>
        /// <returns></returns>
        private string ProcessUploadedFile() {
            //Уникальное имя файла
            string uniqueFileName = null;
            if (inputFile != null)
            {
                //Папка в которую загружается файл
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "files");
                //Генерация уникального имени файла
                uniqueFileName = Guid.NewGuid().ToString() +"_" + inputFile.FileName;

                //Получение полного пути к файлу
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                //Сохранение нового файла
                using (var fs = new FileStream(filePath, FileMode.Create)) {
                    inputFile.CopyTo(fs);
                }
                
            }            
            return uniqueFileName;
        }

        private int ProcessDeleteFile(string uniqueFileName) {        
            if (uniqueFileName != null)
            {
                //Получение пути к файлу
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "files", uniqueFileName);
                //Удаление файла
                System.IO.File.Delete(filePath);
                return 0;
            }
            else return -1;
        }

        public InputFileModel(IWebHostEnvironment webHostEnvironment) {
            _webHostEnvironment = webHostEnvironment;
        }
    }
}
