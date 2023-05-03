using AntColonyClient.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AntColonyClient.Service
{
    public class SQLUserTask : IUserTaskRepository
    {
        private readonly AppDbContext _context;

        public SQLUserTask(AppDbContext context)
        {
            //Зависимость БД
            this._context = context;
        }

        public async Task<UserTask> AddTask(UserTask newUserTask)
        {
            _context.Database.ExecuteSqlRaw("AddNewUserTask @name, @createData, @inputMethod",
                                             new SqlParameter("@name", newUserTask.Name),
                                             new SqlParameter("@createData", newUserTask.Create_Data),
                                             new SqlParameter("@inputMethod", newUserTask.InputMethod));
            return newUserTask;
        }

        public async Task<int> DeleteTask(int id)
        {
            /*var userTaskToDelete = _context.UserTasks.Find(id);
            if (userTaskToDelete != null) {
                _context.UserTasks.Remove(userTaskToDelete);
                _context.SaveChanges();
            }
            */
            return _context.Database.ExecuteSqlRaw("DeleteUserTask @id", new SqlParameter("id", id));
            
        }

        public async Task<IEnumerable<UserTask>> GetAllTasks()
        {
            return _context.UserTasks.FromSqlRaw<UserTask>("GetAllTasks").ToList();
        }

        public async Task<UserTask> GetTaskById(int id)
        {
            return _context.UserTasks.FromSqlRaw<UserTask>("GetTaskById @id", new SqlParameter("@id", id)).ToList().FirstOrDefault(); //_context.UserTasks.Find(id);
        }

        public async Task<int> GetTaskCount()
        {
            return _context.UserTasks.Count();
        }

        public async Task<UserTask> UpdateTask(UserTask updateUserTask)
        {
            var taskToUpdate = _context.UserTasks.Attach(updateUserTask);
            taskToUpdate.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return updateUserTask;
        }
    }
}
