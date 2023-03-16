using AntColonyClient.Models;
using AntColonyClient.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AntColonyClient.Pages.ClientPages
{
    public class TaskDetailsModel : PageModel
    {
        private readonly IUserTaskRepository _userTaskRepository;

        public TaskDetailsModel(IUserTaskRepository userTaskRepository) {
            //Внедрение зависимостей интерфейса
            _userTaskRepository = userTaskRepository;
        }

        public UserTask userTask { get; private set; }

        public IActionResult OnGet(int id)
        {
            userTask = _userTaskRepository.GetTaskById(id);
            //Переадрессация в случае ошибки
            if (userTask == null)
            {
                return RedirectToPage("/NotFound");
            }
            else return Page();
        }
    }
}
