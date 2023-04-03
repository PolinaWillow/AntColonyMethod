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
        IEnumerable<TaskParams> GetAllTaskParams(int idTask);

        TaskParams GetTaskParamById(int id);

        TaskParams UpdateTaskParam(TaskParams updateTaskParam);

        TaskParams AddTaskParam(TaskParams newTaskParam);

        int DeleteTaskParam(int id);

        int GetParamCount();
    }
}
