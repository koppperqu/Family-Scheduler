using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CIS341_Project.Models
{
    public class Workload
    {
        public int WorkloadID { get; set; } //PK
        [StringLength(100)]
        [DisplayName("Workload Description")]
        public string Description { get; set; } = string.Empty;
        [Range(0, 10)]//0-10 scale of difficulty to do
        [DisplayName("Workload Value")]
        public int Value { get; set; }

        // Navigation properties

        [DisplayName("Tasks By Workload")]
        public ICollection<Task> TasksByWorkload { get; set; } = null!;
    }
}
