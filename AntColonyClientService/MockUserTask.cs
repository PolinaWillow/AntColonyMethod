﻿using AntColonyClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyClient.Service
{
    public class MockUserTask : IUserTaskRepository
    {
        private List<UserTask> _userTasks;

        public MockUserTask() {
            _userTasks = new List<UserTask>()
            {
                new UserTask()
                {
                    Id = 0, Name="Task 1", Create_Data="28.02.2023_15.37", InputMethod=Enum_InputMethods.Manual
                },
                new UserTask()
                {
                    Id = 1, Name="Task 2", Create_Data="27.02.2023_15.37", InputMethod=Enum_InputMethods.From_File
                },
                new UserTask()
                {
                    Id = 2, Name="Task 3", Create_Data="26.02.2023_15.37", InputMethod=Enum_InputMethods.From_File
                },
                new UserTask()
                {
                    Id = 3, Name="Task 4", Create_Data="25.02.2023_15.37", InputMethod=Enum_InputMethods.Manual
                },
                new UserTask()
                {
                    Id = 4, Name="Task 5", Create_Data="24.02.2023_15.37", InputMethod=Enum_InputMethods.From_File
                },
                new UserTask()
                {
                    Id = 5, Name="Task 6", Create_Data="23.02.2023_15.37", InputMethod=Enum_InputMethods.Manual
                },
                new UserTask()
                {
                    Id = 6, Name="Task 7", Create_Data="28.02.2023_15.37", InputMethod=Enum_InputMethods.From_File
                },
            };
        }

        public async Task<UserTask> AddTask(UserTask newUserTask)
        {
            newUserTask.Id = _userTasks.Max(x=>x.Id)+1;
            DateTime dateTime = DateTime.UtcNow.Date;
            newUserTask.Create_Data = dateTime.ToString("dd/MM/yyyy");
            _userTasks.Add(newUserTask);
            return newUserTask;
        }

        public async Task<int> DeleteTask(int id)
        {
            UserTask deleteTask = _userTasks.FirstOrDefault(x=>x.Id==id);
            if (deleteTask != null) {
                _userTasks.Remove(deleteTask);
            }
            return 0;
        }

        public async Task<IEnumerable<UserTask>> GetAllTasks()
        {
            return _userTasks;
        }

        public async Task<UserTask> GetTaskById(int id)
        {
            //Возврат единственного пользователя, у которого id == заданному
            return _userTasks.FirstOrDefault(x => x.Id == id);
        }

        public async Task<int> GetTaskCount()
        {
            throw new NotImplementedException();
        }

        public async Task<UserTask> UpdateTask(UserTask updateUserTask)
        {
            UserTask userTask = _userTasks.FirstOrDefault(x=>x.Id==updateUserTask.Id);
            if (userTask != null) {
                userTask.Name = updateUserTask.Name;
                userTask.InputMethod = updateUserTask.InputMethod;
            }
            return userTask;
        }
    }
}
