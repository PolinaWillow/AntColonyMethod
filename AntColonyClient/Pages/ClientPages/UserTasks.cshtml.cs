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
        
        public IEnumerable<UserTask> UserTasks { get; set; }

        [BindProperty]
        public UserTask userTask { get; set; } 

        public void OnGet()
        {
            //Заполнение модели
            UserTasks = _db.GetAllTasks();
        }

        public IActionResult OnPostDelete(int id) {
            userTask =  _db.GetTaskById(id);
            if (userTask != null) {
                userTask = _db.DeleteTask(id);
            }

            return RedirectToPage();
        }
    }
}
