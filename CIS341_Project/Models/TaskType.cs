using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CIS341_Project.Models
{
    public class TaskType
    {
        public int TaskTypeID { get; set; } //PK
        [StringLength(100)]
        [DisplayName("Task Type Description")]
        public string Description { get; set; } = string.Empty;

        // Navigation properties
        [DisplayName("Tasks By Task Type")]
        public ICollection<Task> TasksByTaskType { get; set; } = null!;
    }
}
