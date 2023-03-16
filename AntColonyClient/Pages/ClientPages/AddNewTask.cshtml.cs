using AntColonyClient.Models;
using AntColonyClient.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AntColonyClient.Pages.ClientPages
{
    public class AddNewTaskModel : PageModel
    {
        private readonly IUserTaskRepository _userTaskRepository;

        public AddNewTaskModel(IUserTaskRepository userTaskRepository)
        {
            //Внедрение зависимостей интерфейса
            _userTaskRepository = userTaskRepository;
        }

        [BindProperty]
        public UserTask UserTask { get; set; }

        public IActionResult OnGet()
        {
            UserTask = new UserTask();
            return Page();
        }

        public IActionResult OnPost() {

            UserTask = _userTaskRepository.AddTask(UserTask);
            Console.WriteLine(UserTask);
            return RedirectToPage("UserTasks");
        }
    }
}
