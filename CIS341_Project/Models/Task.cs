using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CIS341_Project.Models
{
    public class Task
    {
        public int TaskID { get; set; } //PK
        [StringLength(100)]
        [DisplayName("Task Description")]
        public string Description { get; set; } = string.Empty;
        public int FrequencyID { get; set; } //FK
        public int TaskTypeID { get; set; } //FK
        public int WorkloadID { get; set; } //FK

        // Navigation properties
        [DisplayName("Frequency")]
        public Frequency Frequency { get; set; } = null!;
        [DisplayName("Task Type")]
        public TaskType TaskType { get; set; } = null!;
        [DisplayName("Workload")]
        public Workload Workload { get; set; } = null!;
        [DisplayName("Assignments By Task")]
        public ICollection<Assignment> AssignmentsByTask { get; set; } = null!;

    }
}
