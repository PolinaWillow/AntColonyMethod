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

        public async Task<IActionResult> OnGetAsync()
        {
            UserTask = new UserTask();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {               
                UserTask = await _userTaskRepository.AddTask(UserTask);
                return RedirectToPage("UserTasks");
            }
            return Page();

        }
    }
}
