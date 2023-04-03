using AntColonyClient.Models;
using AntColonyClient.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AntColonyClient.Pages.ClientPages
{
    public class UserTasksModel : PageModel
    {
        //������ � ��
        private readonly IUserTaskRepository _db;
        
        public UserTasksModel(IUserTaskRepository db) {
            //��������� ����������� ���� ������
            _db = db;
        }
        
        public IEnumerable<UserTask> UserTasks { get; set; }

        [BindProperty]
        public UserTask userTask { get; set; } 
        public int UserTaskCount { get; set; }

        public void OnGet()
        {
            //���������� ������
            UserTasks = _db.GetAllTasks();
            UserTaskCount = _db.GetTaskCount();
        }

        public IActionResult OnPostDelete(int id) {
            userTask =  _db.GetTaskById(id);
            if (userTask != null) {
                _db.DeleteTask(id);
            }

            return RedirectToPage();
        }
    }
}
