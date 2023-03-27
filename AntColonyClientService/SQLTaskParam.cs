using AntColonyClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyClient.Service
{
    public class SQLTaskParam : ITaskParamsRepository
    {
        private readonly AppDbContext _context;

        public SQLTaskParam(AppDbContext context) {
            //Зависимость БД
            this._context = context;
        }

        public TaskParams AddTaskParam(TaskParams newTaskParam)
        {
            _context.TaskParams.Add(newTaskParam);
            _context.SaveChanges();
            return newTaskParam;
        }

        public TaskParams DeleteTaskParam(int id)
        {
            var userParamToDelete = _context.TaskParams.Find(id);
            if (userParamToDelete != null)
            {
                _context.TaskParams.Remove(userParamToDelete);
                _context.SaveChanges();
            }
            return userParamToDelete;
        }

        public IEnumerable<TaskParams> GetAllTaskParams()
        {
            return _context.TaskParams;
        }

        public TaskParams GetTaskParamById(int id)
        {
            return _context.TaskParams.Find(id);
        }

        public TaskParams UpdateTaskParam(TaskParams updateTaskParam)
        {
            var paramToUpdate = _context.TaskParams.Attach(updateTaskParam);
            paramToUpdate.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return updateTaskParam;
        }
    }
}
