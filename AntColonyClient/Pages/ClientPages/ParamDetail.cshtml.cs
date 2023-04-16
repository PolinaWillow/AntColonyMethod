using AntColonyClient.Models;
using AntColonyClient.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AntColonyClient.Pages.ClientPages
{
    public class ParamDetailModel : PageModel
    {
        private readonly IUserTaskRepository _userTaskRepository;
        private readonly ITaskParamsRepository _taskParamRepository;

        public ParamDetailModel(IUserTaskRepository userTaskRepository, ITaskParamsRepository taskParamsRepository) 
        {
            //Внедрение зависимостей интерфейса
            _userTaskRepository = userTaskRepository;
            _taskParamRepository = taskParamsRepository;
        }
        public TaskParams taskParam { get; private set; }
        public IActionResult OnGet(int id)
        {
            taskParam = _taskParamRepository.GetTaskParamById(id);
            //Переадрессация в случае ошибки
            if (taskParam == null)
            {
                return RedirectToPage("/NotFound");
            }
            else
            {
                return Page();
            }
        }
    }
}
