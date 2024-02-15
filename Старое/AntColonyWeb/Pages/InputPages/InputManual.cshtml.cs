using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AntColonyWeb.DataModel;

namespace AntColonyWeb.Pages.InputPages
{
    public class InputManualModel : PageModel
    {
        /// <summary>
        /// ���� ����������
        /// </summary>
        public string[] typesParam = new[] { "���������", "��������", "�������� � �����", "Bool" };
        /// <summary>
        /// Id ��� ���������� �������� ������� ����
        /// </summary>
        public string[] idsTypesValue = new[] { "stringParam", "numParam", "numWithStepParam", "boolParam" };


        /// <summary>
        /// ���������� ����������
        /// </summary>
        public int ParamCount { get; set; }

        [BindProperty]
        public ConfigParam newParam { get; set; }

        [BindProperty]
        public List<ConfigParam> Params { get; set; }

        /// <summary>
        /// ����������� ����������� ����������
        /// </summary>
        public List<ConfigParam> DisplayedParams { get; set; }

        //���������� ������ ��������� ���������
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
                    numParam= 1, typeParam = "���������", valueCountParam = 2, valuesParam= {"�������", "C�����" }
                },
                new ConfigParam()
                {
                    numParam= 2, typeParam = "��������", valueCountParam = 3, valuesParam=  {"122", "1883","32" }
                },
                new ConfigParam()
                {
                    numParam= 3, typeParam = "Bool", valueCountParam = 1, valuesParam= {"True"}
                },
                 new ConfigParam()
                {
                   numParam= 4, typeParam = "��������", valueCountParam = 3, valuesParam=  {"122", "1883","32" }
                },
                  new ConfigParam()
                {
                   numParam= 5, typeParam = "��������", valueCountParam = 3, valuesParam=  {"122", "1883","32" }
                }
            };
            DisplayedParams = new();
        }
    }
}
