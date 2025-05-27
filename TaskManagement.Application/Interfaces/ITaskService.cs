using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces
{
    public interface ITaskService
    {
        List<TaskDto> GetAllTasks();
        List<TaskDto> GetAllTasks(int page, int pageSize);
        TaskDto CreateTask(CreateTaskDto createDto);
        TaskDto GetTaskById(int taskId);
        TaskDto UpdateTask(int taskId, UpdateTaskDto dto);
        void DeleteTask(int taskId);
        void AssignTask(int taskId, int userId);
        List<TaskDto> GetTaskByUser(int userId);

    }
}
