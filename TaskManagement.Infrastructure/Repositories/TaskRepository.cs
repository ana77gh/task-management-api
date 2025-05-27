using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly List<TaskEntity> _tasks;
        private int _nextId = 1;

        public TaskRepository()
        {
            // Seed data dummy
            _tasks = new List<TaskEntity>
            {
                new TaskEntity { 
                    Id = 1, 
                    Title = "Task 1",
                    Description = "Desc Task 1",
                    DueDate = DateTime.Now.AddDays(2),
                    Priority = Priority.Medium,
                    Status = Status.Open,
                    AssignedUserId = null
                },
                new TaskEntity {
                    Id = 2,
                    Title = "Task 2",
                    Description = "Desc Task 2",
                    DueDate = DateTime.Now.AddDays(3),
                    Priority = Priority.Medium,
                    Status = Status.Open,
                    AssignedUserId = null
                },
                new TaskEntity {
                    Id = 3,
                    Title = "Task 3",
                    Description = "Desc Task 3",
                    DueDate = DateTime.Now.AddDays(4),
                    Priority = Priority.Medium,
                    Status = Status.Open,
                    AssignedUserId = 7
                },
                new TaskEntity {
                    Id = 4,
                    Title = "Task 4",
                    Description = "Desc Task 4",
                    DueDate = DateTime.Now.AddDays(5),
                    Priority = Priority.High,
                    Status = Status.Completed,
                    AssignedUserId = null
                },
                new TaskEntity {
                    Id = 5,
                    Title = "Task 5",
                    Description = "Desc Task 5",
                    DueDate = DateTime.Now.AddDays(1),
                    Priority = Priority.Medium,
                    Status = Status.Open,
                    AssignedUserId = 2
                },
                new TaskEntity {
                    Id = 6,
                    Title = "Task 6",
                    Description = "Desc Task 6",
                    DueDate = DateTime.Now.AddDays(2),
                    Priority = Priority.Low,
                    Status = Status.Open,
                    AssignedUserId = 3
                },
                new TaskEntity {
                    Id = 7,
                    Title = "Task 7",
                    Description = "Desc Task 7",
                    DueDate = DateTime.Now.AddDays(3),
                    Priority = Priority.High,
                    Status = Status.InProgress,
                    AssignedUserId = 4
                },
                new TaskEntity {
                    Id = 8,
                    Title = "Task 8",
                    Description = "Desc Task 8",
                    DueDate = DateTime.Now.AddDays(4),
                    Priority = Priority.Medium,
                    Status = Status.Completed,
                    AssignedUserId = null
                },
                new TaskEntity {
                    Id = 9,
                    Title = "Task 9",
                    Description = "Desc Task 9",
                    DueDate = DateTime.Now.AddDays(5),
                    Priority = Priority.Low,
                    Status = Status.Open,
                    AssignedUserId = 5
                },
                new TaskEntity {
                    Id = 10,
                    Title = "Task 10",
                    Description = "Desc Task 10",
                    DueDate = DateTime.Now.AddDays(1),
                    Priority = Priority.High,
                    Status = Status.Open,
                    AssignedUserId = null
                },
                new TaskEntity {
                    Id = 11,
                    Title = "Task 11",
                    Description = "Desc Task 11",
                    DueDate = DateTime.Now.AddDays(2),
                    Priority = Priority.Medium,
                    Status = Status.InProgress,
                    AssignedUserId = 6
                },
                new TaskEntity {
                    Id = 12,
                    Title = "Task 12",
                    Description = "Desc Task 12",
                    DueDate = DateTime.Now.AddDays(3),
                    Priority = Priority.High,
                    Status = Status.Completed,
                    AssignedUserId = null
                },
                new TaskEntity {
                    Id = 13,
                    Title = "Task 13",
                    Description = "Desc Task 13",
                    DueDate = DateTime.Now.AddDays(4),
                    Priority = Priority.Low,
                    Status = Status.Open,
                    AssignedUserId = 7
                },
                new TaskEntity {
                    Id = 14,
                    Title = "Task 14",
                    Description = "Desc Task 14",
                    DueDate = DateTime.Now.AddDays(5),
                    Priority = Priority.Medium,
                    Status = Status.InProgress,
                    AssignedUserId = null
                },
                new TaskEntity {
                    Id = 15,
                    Title = "Task 15",
                    Description = "Desc Task 15",
                    DueDate = DateTime.Now.AddDays(6),
                    Priority = Priority.High,
                    Status = Status.Open,
                    AssignedUserId = 8
                }

            };
        }

        public List<TaskEntity> GetAll() => _tasks;

        public TaskEntity GetById(int id)
        {
            return _tasks.FirstOrDefault(t => t.Id == id);
        }

        public TaskEntity Add(TaskEntity task)
        {
            task.Id = _nextId++;
            _tasks.Add(task);
            return task;
        }

        public TaskEntity Update(TaskEntity task)
        {
            var existing = _tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existing == null) throw new Exception("Task not found");

            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.DueDate = task.DueDate;
            existing.Priority = task.Priority;
            existing.Status = task.Status;
            existing.AssignedUserId = task.AssignedUserId;

            return existing;

        }

        public void Delete(int id)
        {
            var task = _tasks.FirstOrDefault(t=>t.Id == id);
            if (task == null) throw new Exception("Task not found");

            _tasks.Remove(task);
        }

    }
}
