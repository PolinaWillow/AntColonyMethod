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

        //Получение деталей задачи
        public UserTask userTask { get; private set; }

        //Получение параметров задачи
        public TaskParams userTaskParams { get; private set; }

        public int paramCount = 0;

        //Добавление параметра
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
                TaskParams = _taskParamRepository.GetAllTaskParams(userTask.Id);
                paramCount = _taskParamRepository.GetParamCount(userTask.Id);


                return Page();
            }
            
        }

        public IActionResult OnPost()
        {
            string url = Url.Page("TaskDetails", new { id = TaskParam.IdTask });
            if (ModelState.IsValid)
            {
                TaskParam = _taskParamRepository.AddTaskParam(TaskParam);
                return Redirect(url ?? "NotFound");
            }
            return Redirect(url ?? "NotFound");
        }

        public IActionResult OnPostDelete(int id)
        {
            userTaskParams = _taskParamRepository.GetTaskParamById(id);
            int IdTask=0;
            if (userTaskParams != null)
            {
                IdTask = userTaskParams.IdTask;
                _taskParamRepository.DeleteTaskParam(id);
            }
            string url = Url.Page("TaskDetails", new { id = IdTask });
            return Redirect(url ?? "NotFound");
        }
    }
}
