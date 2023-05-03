using AntColonyClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyClient.Service
{
    public interface IUserTaskRepository
    {
        Task<IEnumerable<UserTask>>  GetAllTasks();

        Task<UserTask> GetTaskById(int id);

        Task<UserTask> UpdateTask(UserTask updateUserTask);

        Task<UserTask> AddTask(UserTask newUserTask);

        Task<int> DeleteTask(int id);

        Task<int> GetTaskCount();
    }
}
