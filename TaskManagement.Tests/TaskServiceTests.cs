using Xunit;
using Moq;
using FluentAssertions;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Services;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Domain.Entities;
using TaskManagement.Application.Exceptions;
using TaskManagement.Domain.Enums;
using TaskManagement.Application.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace TaskManagement.Tests
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly Mock<ILogger<TaskService>> _loggerMock;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _loggerMock = new Mock<ILogger<TaskService>>();
            _taskService = new TaskService(_taskRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void GetAllTasks_WithPagination_ReturnsCorrectSubset()
        {
            var tasks = Enumerable.Range(1, 20)
                .Select(i => new TaskEntity { Id = i, Title = $"Task {i}" })
                .ToList();

            _taskRepositoryMock.Setup(r => r.GetAll()).Returns(tasks);

            var result = _taskService.GetAllTasks(page: 2, pageSize: 5);

            result.Should().HaveCount(5);
            result.First().Id.Should().Be(6); // karena page 2
        }


        [Fact]
        public void GetTaskById_TaskExists_ReturnsTaskDto()
        {
            DateTime dueDate = DateTime.Now.AddDays(2);
            // Arrange
            var task = new TaskEntity
            {
                Id = 1,
                Title = "Task 1",
                Description = "Test Desc 1",
                DueDate = dueDate,
                Priority = Priority.Medium,
                Status = Status.Completed
            };
            _taskRepositoryMock.Setup(repo => repo.GetById(1)).Returns(task);

            // Act
            var result = _taskService.GetTaskById(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Title.Should().Be("Task 1");
            result.Priority.Should().Be(Priority.Medium);
            result.DueDate.Should().Be(dueDate);
            result.Status.Should().Be(Status.Completed);
        }

        [Fact]
        public void GetTaskById_TaskDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            _taskRepositoryMock.Setup(repo => repo.GetById(99)).Returns((TaskEntity)null);

            // Act
            var act = () => _taskService.GetTaskById(99);

            // Assert
            act.Should().Throw<NotFoundException>()
               .WithMessage("Task dengan id 99 tidak ditemukan.");
        }

        [Fact]
        public void CreateTask_ValidInput_ReturnsCreatedTask()
        {
            // Arrange
            var createDto = new CreateTaskDto
            {
                Title = "New Task",
                Description = "This is a new task",
                DueDate = DateTime.UtcNow.AddDays(3),
                Priority = "Medium",
                Status = "Open"
            };

            var savedEntity = new TaskEntity
            {
                Id = 1,
                Title = createDto.Title,
                Description = createDto.Description,
                DueDate = createDto.DueDate,
                Priority = Priority.Medium,
                Status = Status.Open
            };

            _taskRepositoryMock.Setup(repo => repo.Add(It.IsAny<TaskEntity>()))
                               .Returns(savedEntity);

            // Act
            var result = _taskService.CreateTask(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Title.Should().Be("New Task");
            result.Priority.Should().Be(Priority.Medium);
            result.Status.Should().Be(Status.Open);
        }

        [Fact]
        public void CreateTask_InvalidDueDate_ThrowsValidationException()
        {
            // Arrange
            var createDto = new CreateTaskDto
            {
                Title = "Invalid Task",
                Description = "This task has past due date",
                DueDate = DateTime.UtcNow.AddDays(-1),
                Priority = "Low",
                Status = "Open"
            };

            // Act
            Action act = () => _taskService.CreateTask(createDto);

            // Assert
            act.Should().Throw<ValidationException>()
               .WithMessage("*tidak boleh di masa lalu*");
        }

        [Fact]
        public void UpdateTask_ValidInput_UpdatesAndReturnsTask()
        {
            // Arrange
            var existingTask = new TaskEntity
            {
                Id = 1,
                Title = "Old Title",
                Description = "Old Description",
                DueDate = DateTime.UtcNow.AddDays(2),
                Priority = Priority.Low,
                Status = Status.Open
            };

            var updateDto = new UpdateTaskDto
            {
                Title = "Updated Title",
                Description = "Updated Description",
                DueDate = DateTime.UtcNow.AddDays(5),
                Priority = "High",
                Status = "InProgress"
            };

            var updatedEntity = new TaskEntity
            {
                Id = 1,
                Title = updateDto.Title,
                Description = updateDto.Description,
                DueDate = updateDto.DueDate,
                Priority = Priority.High,
                Status = Status.InProgress
            };

            _taskRepositoryMock.Setup(repo => repo.GetById(1)).Returns(existingTask);
            _taskRepositoryMock.Setup(repo => repo.Update(It.IsAny<TaskEntity>())).Returns(updatedEntity);

            // Act
            var result = _taskService.UpdateTask(updatedEntity.Id, updateDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Title.Should().Be("Updated Title");
            result.Priority.Should().Be(Priority.High);
            result.Status.Should().Be(Status.InProgress);
        }

        [Fact]
        public void UpdateTask_TaskNotFound_ThrowsNotFoundException()
        {
            // Arrange
            _taskRepositoryMock.Setup(repo => repo.GetById(99)).Returns((TaskEntity)null);

            var dto = new UpdateTaskDto
            {
                Title = "New Title",
                Description = "Desc",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = "Medium",
                Status = "Open"
            };

            // Act
            var act = () => _taskService.UpdateTask(99, dto);

            // Assert
            act.Should().Throw<NotFoundException>()
               .WithMessage("Task dengan id 99 tidak ditemukan.");
        }

        [Fact]
        public void DeleteTask_TaskExists_DeletesTask()
        {
            // Arrange
            int idDel = 1;
            var existingTask = new TaskEntity
            {
                Id = idDel,
                Title = "Sample Task",
                Description = "Some task",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = Priority.High,
                Status = Status.Open
            };

            _taskRepositoryMock.Setup(repo => repo.GetById(idDel)).Returns(existingTask);

            _taskRepositoryMock.Setup(repo => repo.Delete(idDel));

            // Act
            _taskService.DeleteTask(idDel);

            //Assert
            _taskRepositoryMock.Verify(repo => repo.GetById(idDel), Times.Once);
            _taskRepositoryMock.Verify(repo => repo.Delete(idDel), Times.Once);
            
        }

        [Fact]
        public void DeleteTask_TaskNotFound_ThrowsNotFoundException()
        {
            // Arrange
            int idDel = 29;
            _taskRepositoryMock.Setup(repo => repo.GetById(idDel)).Returns((TaskEntity)null);

            // Act
            var act = () => _taskService.DeleteTask(idDel);

            //Assert
            act.Should().Throw<NotFoundException>().WithMessage($"Task dengan id {idDel} tidak ditemukan.");
        }

        [Fact]
        public void AssignTaskToUser_ValidTask_AssignSuccessfully()
        {
            //Arrange
            var task = new TaskEntity
            {
                Id = 1,
                Title = "Old Title",
                Description = "Old Description",
                DueDate = DateTime.UtcNow.AddDays(2),
                Priority = Priority.Low,
                Status = Status.Open,
                AssignedUserId = null
            };

            _taskRepositoryMock.Setup(repo => repo.GetById(task.Id)).Returns(task);

            //Act
            _taskService.AssignTask(1, 104);

            //Assert
            task.AssignedUserId.Should().Be(104);
            _taskRepositoryMock.Verify(r=>r.Update(It.Is<TaskEntity>(t=>t.Id == 1 && t.AssignedUserId == 104)), Times.Once());
        }

        [Fact]
        public void AssignTaskToUser_TaskNotFound_ThrowsNotFoundException()
        {
            //Arrange
            int taskId = 10;
            _taskRepositoryMock.Setup(r => r.GetById(taskId)).Returns((TaskEntity)null);

            //Act
            var act = () => _taskService.AssignTask(10, 105);

            //Assert
            act.Should().Throw<NotFoundException>().WithMessage($"Task dengan id {taskId} tidak ditemukan.");


        }

        [Fact]
        public void GetTasksByUserId_UserHasTasks_ReturnAssignedTasks()
        {
            //Arrange
            var userId = 99;

            var tasks = new List<TaskEntity>
            {
                new TaskEntity { Id = 1, Title = "Task A", AssignedUserId = 99 },
                new TaskEntity { Id = 2, Title = "Task B", AssignedUserId = 88 },
                new TaskEntity { Id = 3, Title = "Task C", AssignedUserId = 99 }
            };

            _taskRepositoryMock.Setup(r=>r.GetAll()).Returns(tasks);

            //Act
            var result = _taskService.GetTaskByUser(userId);

            //Assert
            result.Should().HaveCount(2);
            result.All(t => t.AssignedUserId == userId).Should().BeTrue();


        }
    }

}
