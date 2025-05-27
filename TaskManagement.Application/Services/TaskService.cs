using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Exceptions;
using TaskManagement.Application.Interfaces;
using System.ComponentModel.DataAnnotations;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using Microsoft.Extensions.Logging;


namespace TaskManagement.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public List<TaskDto> GetAllTasks()
        {
            var tasks = _taskRepository.GetAll();
            return tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Priority = t.Priority,
                Status = t.Status,
                AssignedUserId = t.AssignedUserId
            }).ToList();
        }

        public List<TaskDto> GetAllTasks(int page, int pageSize)
        {
            var tasks = _taskRepository.GetAll()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    Priority = t.Priority,
                    Status = t.Status,
                    AssignedUserId = t.AssignedUserId
                })
                .ToList();

            return tasks;
        }


        public TaskDto GetTaskById(int id)
        {
            var task = _taskRepository.GetById(id);
            if (task == null)
                throw new NotFoundException($"Task dengan id {id} tidak ditemukan.");

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Status = task.Status,
                AssignedUserId = task.AssignedUserId
            };
        }

        public TaskDto CreateTask(CreateTaskDto createDto)
        {
            if (!Enum.TryParse<Priority>(createDto.Priority, true, out var priority))
            {
                _logger.LogWarning("Create failed: invalid priority.");
                throw new ValidationException("Prioritas tidak valid.");
            }

            if (!Enum.TryParse<Status>(createDto.Status, true, out var status))
            {
                _logger.LogWarning("Create failed: invalid status.");
                throw new ValidationException("Status tidak valid.");
            }               

            if (createDto.DueDate < DateTime.Now)
            {
                _logger.LogWarning("Create failed: due date must not be in the past");
                throw new ValidationException("Due date tidak boleh di masa lalu.");
            }
                

            if (createDto.DueDate < DateTime.Today)
            {
                _logger.LogWarning("Create failed: due date must not be in the past");
                throw new ValidationException("Due date tidak boleh di masa lalu.");
            }               

            var newTask = new TaskEntity
            {
                Title = createDto.Title,
                Description = createDto.Description,
                DueDate = createDto.DueDate,
                Priority = priority,
                Status = status,
                AssignedUserId = null
                
            };

            var created = _taskRepository.Add(newTask);
            _logger.LogInformation("Task created: {Title}", created.Title);


            return new TaskDto
            {
                Id = created.Id,
                Title = created.Title,
                Description = created.Description,
                DueDate = created.DueDate,
                Priority = created.Priority,
                Status = created.Status,
                AssignedUserId = created.AssignedUserId
            };
        }

    public TaskDto UpdateTask(int id, UpdateTaskDto dto)
        {
            var task = _taskRepository.GetById(id);

            if (task == null)
            {
                _logger.LogWarning("Update failed: task with id {Id} not found.", id);
                throw new NotFoundException($"Task dengan id {id} tidak ditemukan.");
            }
                

            if(dto.Title is not null) task.Title = dto.Title;
            if (dto.Description is not null) task.Description = dto.Description;
            if (dto.DueDate < DateTime.Now)
            {
                _logger.LogWarning("Update failed: due date must not be in the past");
                throw new ValidationException("Due date tidak boleh di masa lalu.");
            }
               

            task.DueDate = dto.DueDate;

            if(!string.IsNullOrEmpty(dto.Priority))
                task.Priority = Enum.Parse<Priority>(dto.Priority, true);

            if (!string.IsNullOrEmpty(dto.Status))
                task.Status = Enum.Parse<Status>(dto.Status, true);

            var updated = _taskRepository.Update(task);
            _logger.LogInformation("Task with id {Id} updated successfully.", updated.Id);

            return new TaskDto
            {
                Id = updated.Id,
                Title = updated.Title,
                Description = updated.Description,
                DueDate = updated.DueDate,
                Priority = updated.Priority,
                Status = updated.Status,
                AssignedUserId = updated.AssignedUserId
            };
        }

        public void DeleteTask(int taskId)
        {
            var task = _taskRepository.GetById(taskId);
            if (task == null)
            {
                _logger.LogWarning("Delete failed: task with id {Id} not found.", taskId);
                throw new NotFoundException($"Task dengan id {taskId} tidak ditemukan.");
            } 

            _taskRepository.Delete(taskId);
            _logger.LogInformation("Task with id {Id} deleted successfully.", taskId);

        }

        public void AssignTask(int taskId, int userId)
        {
            var task = _taskRepository.GetById(taskId);
            if(task == null)
            {
                _logger.LogWarning("Delete failed: task with id {Id} not found.", taskId);
                throw new NotFoundException($"Task dengan id {taskId} tidak ditemukan.");
            }

            task.AssignedUserId = userId;
            _taskRepository.Update(task);
            _logger.LogInformation("Task with id {Id} assigned successfully.", taskId);

        }

        public List<TaskDto> GetTaskByUser(int userId)
        {
            var tasks = _taskRepository.GetAll()
                .Where(t => t.AssignedUserId == userId)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    Priority = t.Priority,
                    Status = t.Status,
                    AssignedUserId = t.AssignedUserId
                })
                .ToList();

            return tasks;

            }
        }

}

