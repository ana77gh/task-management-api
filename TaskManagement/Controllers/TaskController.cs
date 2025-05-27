using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.DTOs;
using System.ComponentModel.DataAnnotations;
using TaskManagement.Application.Exceptions;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public ActionResult<List<TaskDto>> GetTasks([FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Page and PageSize must be greater than zero.");
            }

            var tasks = _taskService.GetAllTasks(page, pageSize);
            return Ok(tasks);
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var task = _taskService.GetTaskById(id);
                return Ok(task);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CreateTask([FromBody] CreateTaskDto dto)
        {
            try
            {
                var createdTask = _taskService.CreateTask(dto);
                return CreatedAtAction(nameof(GetById), new
                { id = createdTask.Id },createdTask);
            }
            catch(ValidationException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateTaskDto dto)
        {
            var updatedTask = _taskService.UpdateTask(id, dto);
            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _taskService.DeleteTask(id);
                return NoContent(); // 204
            }
            catch(Exception ex)
            {
                return NotFound(new { error = ex.Message });
            }

        }

        [HttpPut("{taskId}/assign/{userId}")]
        public IActionResult AssignTaskToUser(int taskId, int userId)
        {
            _taskService.AssignTask(taskId, userId);
            
            return NoContent();
            
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetTaskByUser(int userId)
        {
            var tasks =_taskService.GetTaskByUser(userId);
            return Ok(tasks);
        }
    }
}
