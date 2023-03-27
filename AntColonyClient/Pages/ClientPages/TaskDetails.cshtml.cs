using AntColonyClient.Models;
using AntColonyClient.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AntColonyClient.Pages.ClientPages
{
    public class TaskDetailsModel : PageModel
    {
        private readonly IUserTaskRepository _userTaskRepository;
        private readonly ITaskParamsRepository _taskParamRepository;

        public IEnumerable<TaskParams> TaskParams { get; set; }

        public TaskDetailsModel(IUserTaskRepository userTaskRepository, ITaskParamsRepository taskParamsRepository) {
            //Внедрение зависимостей интерфейса
            _userTaskRepository = userTaskRepository;
            _taskParamRepository = taskParamsRepository;
        }

        public UserTask userTask { get; private set; }
        [BindProperty]
        public TaskParams TaskParam { get; set; }

        public IActionResult OnGet(int id)
        {
            userTask = _userTaskRepository.GetTaskById(id);
            //Переадрессация в случае ошибки
            if (userTask == null)
            {
                return RedirectToPage("/NotFound");
            }
            else {
                TaskParams = _taskParamRepository.GetAllTaskParams();
                return Page();
            }
            
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                TaskParam = _taskParamRepository.AddTaskParam(TaskParam);
                return Page();
            }
            return Page();
        }
    }
}
