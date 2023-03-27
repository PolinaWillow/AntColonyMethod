using AntColonyClient.Models;
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

        public UserTask AddTask(UserTask newUserTask)
        {
            _context.UserTasks.Add(newUserTask);
            _context.SaveChanges();
            return newUserTask;
        }

        public UserTask DeleteTask(int id)
        {
            var userTaskToDelete = _context.UserTasks.Find(id);
            if (userTaskToDelete != null) {
                _context.UserTasks.Remove(userTaskToDelete);
                _context.SaveChanges();
            }
            return userTaskToDelete;
        }

        public IEnumerable<UserTask> GetAllTasks()
        {
            return _context.UserTasks;
        }

        public UserTask GetTaskById(int id)
        {
            return _context.UserTasks.Find(id);
        }

        public UserTask UpdateTask(UserTask updateUserTask)
        {
            var taskToUpdate = _context.UserTasks.Attach(updateUserTask);
            taskToUpdate.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return updateUserTask;
        }
    }
}
