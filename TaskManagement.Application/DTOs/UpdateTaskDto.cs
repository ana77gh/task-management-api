using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Attributes;

namespace TaskManagement.Application.DTOs
{
    public class UpdateTaskDto
    {
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }
        [FutureDate(ErrorMessage = "Due date must be in the future.")]
        public DateTime DueDate { get; set; }
        public string? Priority { get; set; }   // Medium, High, etc.
        public string? Status { get; set; }     // Open, InProgress, etc.
    }
}
