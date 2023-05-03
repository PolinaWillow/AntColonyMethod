using AntColonyClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyClient.Service
{
    public interface ITaskParamsRepository
    {
        Task<IEnumerable<TaskParams>> GetAllTaskParams(int idTask);

        Task<TaskParams> GetTaskParamById(int id);

        Task<TaskParams> UpdateTaskParam(TaskParams updateTaskParam);

        Task<TaskParams> AddTaskParam(TaskParams newTaskParam);

        Task<int> DeleteTaskParam(int id);

        Task<int> GetParamCount(int idTask);
    }
}
