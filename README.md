
# Task Management System

A clean-architecture based Task Management API built with .NET 8 as a take-home test assignment.

## ✅ Features

- Create, update, delete tasks
- Assign tasks to users
- Retrieve all tasks or by user
- Data validation (e.g., required fields, due date cannot be in the past)
- In-memory repository (mock database)
- Logging for critical operations
- Unit tests using xUnit, Moq, and FluentAssertions
- Clean architecture with proper layering

## 🏗️ Tech Stack

- .NET 8
- ASP.NET Core Web API
- xUnit (unit testing)
- Moq (mocking dependencies)
- FluentAssertions (clean assertions)

## 🗂️ Project Structure

```
TaskManagement.API             → Presentation Layer (controllers, Program.cs)
TaskManagement.Application    → Application Layer (DTOs, Services, Interfaces)
TaskManagement.Domain         → Domain Layer (Entities, Enums)
TaskManagement.Infrastructure → Infrastructure Layer (Repository implementation)
TaskManagement.Tests          → Unit Tests
```

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Run the project

```bash
dotnet build
dotnet run --project TaskManagement.API
```

Open Swagger UI at: `https://localhost:7093/swagger`

### Run tests

```bash
dotnet test
```

## 📋 API Endpoints

- `GET /api/task?page=1&pageSize=10` → Paginated list of tasks
- `GET /api/task/{id}` → Get task by ID
- `POST /api/task` → Create new task
- `PUT /api/task/{id}` → Update task
- `DELETE /api/task/{id}` → Delete task
- `PUT /api/task/{taskId}/assign/{userId}` → Assign task to a user
- `GET /api/task/user/{userId}` → Get tasks assigned to a user

## 📄 Notes

- No database is used; all data is stored in memory
- No frontend required
- Designed with SOLID principles and Clean Architecture in mind
- Exception handling is done via middleware

---

Feel free to fork or clone the repo and improve upon it.
