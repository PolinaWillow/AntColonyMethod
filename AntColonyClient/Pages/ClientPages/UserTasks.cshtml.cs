using AntColonyClient.Models;
using AntColonyClient.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AntColonyClient.Pages.ClientPages
{
    public class UserTasksModel : PageModel
    {
        //Доступ к БД
        private readonly IUserTaskRepository _db;
        
        public UserTasksModel(IUserTaskRepository db) {
            //Внедрение зависимости базы данных
            _db = db;
        }
        
        [BindProperty]
        public UserTask userTask { get; set; }

        public IEnumerable<UserTask> UserTasks { get; set; }
        public int UserTaskCount { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            //Заполнение модели
             UserTasks = await _db.GetAllTasks();
             UserTaskCount = await _db.GetTaskCount();

             ViewData["view_UserTasks"] = UserTasks;
             ViewData["view_UserTaskCount"] = UserTaskCount;

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id) {
            var userTask = await _db.GetTaskById(id);
            if (userTask != null) {
                await _db.DeleteTask(id);
            }

            return RedirectToPage();
        }
    }
}
