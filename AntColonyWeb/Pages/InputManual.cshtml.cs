using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AntColonyWeb.Pages
{
    public class InputManualModel : PageModel
    {
        public string Message { get; set; }
        /// <summary>
        /// ���������� ����������
        /// </summary>
        public int ParamCount { get; set; } = 0;
        /// <summary>
        /// ���� ������� ������ ���������� (true - ������; false - �� ������)
        /// </summary>
        public bool SaveFlag { get; set; } = false;
        public void OnGet()
        {
            //ParamCount = 3;
        }

        /// <summary>
        /// ��������� ���������� ���������� �� ����� � ��������� ����� ����� ����������
        /// </summary>
        /// <param name="paramCount">���������� ����������</param>
        public void OnPostSaveParamCount(int paramCount)
        {
            SaveFlag = true;
            if (paramCount <= 0) //�������� �� ������������� ��������
            {
                ParamCount = 0;
            }
            else {
                ParamCount = paramCount;
            }

            //��������� ����� ����� ����������
        }

        /// <summary>
        /// ���������� �������� ����������, ��������� �����, �������� �� ������, ����� ����������� ������ �������, ������ �������
        /// </summary>
        public void OnPostStartWork() {
            //������ �������, ����� ���� ��������
        }
    }
}
