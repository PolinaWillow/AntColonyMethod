using AntColonyClient.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

        public async Task<TaskParams> AddTaskParam(TaskParams newTaskParam)
        {
            _context.Database.ExecuteSqlRaw("AddTaskParam @idTask, @numParam, @typeParam",
                                             new SqlParameter("@idTask", newTaskParam.IdTask),
                                             new SqlParameter("@numParam", newTaskParam.NumParam),
                                             new SqlParameter("@typeParam", newTaskParam.TypeParam));
            return newTaskParam;
        }

        public async Task<int> DeleteTaskParam(int id)
        {
            return _context.Database.ExecuteSqlRaw("DeleteTaskParam @id", new SqlParameter("id", id));
        }

        public async Task<IEnumerable<TaskParams>> GetAllTaskParams(int idTask)
        {
            return _context.TaskParams.FromSqlRaw<TaskParams>("GetAllTaskParams @idTask", new SqlParameter("idTask", idTask)).ToList();
        }

        public async Task<int> GetParamCount(int idTask)
        {
            //return _context.TaskParams.FromSqlRaw<TaskParams>("GetParamCount @idTask", new SqlParameter("idTask", idTask)).Count();
            return _context.TaskParams.Count(u => u.IdTask ==idTask);
        }

        public async Task<TaskParams> GetTaskParamById(int id)
        {
            return _context.TaskParams.Find(id);
        }

        public async Task<TaskParams> UpdateTaskParam(TaskParams updateTaskParam)
        {
            var paramToUpdate = _context.TaskParams.Attach(updateTaskParam);
            paramToUpdate.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return updateTaskParam;
        }
    }
}
