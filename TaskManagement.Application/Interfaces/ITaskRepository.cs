using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces
{
    public interface ITaskRepository
    {
        List<TaskEntity> GetAll();

        TaskEntity? GetById(int id);

        TaskEntity Add(TaskEntity task);

        TaskEntity Update(TaskEntity task);

        void Delete(int id);

    }
}
