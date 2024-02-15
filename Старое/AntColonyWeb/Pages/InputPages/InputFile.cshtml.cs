using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AntColonyWeb.Pages.InputPages
{
    public class InputFileModel : PageModel
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        /// <summary>
        /// ���� � �������
        /// </summary>
        [BindProperty]
        public IFormFile inputFile { get; set; }

        /// <summary>
        /// ���� � �����
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
        /// ���������� ������ ����� �� ������
        /// </summary>
        /// <returns></returns>
        private string ProcessUploadedFile() {
            //���������� ��� �����
            string uniqueFileName = null;
            if (inputFile != null)
            {
                //����� � ������� ����������� ����
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "files");
                //��������� ����������� ����� �����
                uniqueFileName = Guid.NewGuid().ToString() +"_" + inputFile.FileName;

                //��������� ������� ���� � �����
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                //���������� ������ �����
                using (var fs = new FileStream(filePath, FileMode.Create)) {
                    inputFile.CopyTo(fs);
                }
                
            }            
            return uniqueFileName;
        }

        private int ProcessDeleteFile(string uniqueFileName) {        
            if (uniqueFileName != null)
            {
                //��������� ���� � �����
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "files", uniqueFileName);
                //�������� �����
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
