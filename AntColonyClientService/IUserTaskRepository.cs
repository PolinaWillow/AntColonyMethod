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
        IEnumerable<UserTask> GetAllTasks();

        UserTask GetTaskById(int id);

        UserTask UpdateTask(UserTask updateUserTask);

        UserTask AddTask(UserTask newUserTask);

        UserTask DeleteTask(int id);
    }
}
