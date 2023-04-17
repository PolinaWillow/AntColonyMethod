using AntColonyClient.Models;
using AntColonyClient.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AntColonyClient.Pages.ClientPages
{
    public class ParamDetailModel : PageModel
    {
        private readonly ITaskParamsRepository _taskParamRepository;
        private readonly IParamValuesRepository _valueParamRepository;

        public ParamDetailModel(IParamValuesRepository valueParamRepository, ITaskParamsRepository taskParamsRepository) 
        {
            //��������� ������������ ����������
            _taskParamRepository = taskParamsRepository;
            _valueParamRepository = valueParamRepository;
        }
        public TaskParams taskParam { get; private set; }
        public IEnumerable<ParamElems> paramValues { get; set; }

        //���������� ��������
        [BindProperty]
        public ParamElems newParamValue { get; set; }
        public IActionResult OnGet(int id)
        {
            taskParam = _taskParamRepository.GetTaskParamById(id);
            //�������������� � ������ ������
            if (taskParam == null)
            {
                return RedirectToPage("/NotFound");
            }
            else
            {
                paramValues = _valueParamRepository.GetAllParamValues(taskParam.Id);
                return Page();
            }
        }

        public IActionResult OnPost()
        {
            string url = Url.Page("ParamDetail", new { id = newParamValue.IdParam });
            if (ModelState.IsValid)
            {
                newParamValue = _valueParamRepository.AddParamValue(newParamValue);
                //TaskParam = _taskParamRepository.AddTaskParam(TaskParam);
                return Redirect(url ?? "NotFound");
            }
            return Redirect(url ?? "NotFound");
        }
    }
}
