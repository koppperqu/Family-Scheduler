using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Build.Framework;
using System.ComponentModel;

namespace CIS341_Project.Models.DTO
{
    public class TaskDTO
    {
        public int TaskID { get; set; }
        [DisplayName("Task")]
        [Required]
        public string Description { get; set; }
        [DisplayName("Frequency")]
        [BindNever]
        public string? FrequencyDescription { get; set; }
        public int FrequencyID { get; set; }
        [DisplayName("Workload")]
        [BindNever]
        public string? WorkloadDescription { get; set; }
        public int WorkloadID { get; set; }
        [DisplayName("Task Type")]
        [BindNever]
        public string? TaskTypeDescription { get; set; }
        public int TaskTypeID { get; set; }

        public TaskDTO() { }
        public TaskDTO(Task task)
        {
            TaskID = task.TaskID;
            Description = task.Description;
            FrequencyID = task.FrequencyID;
            FrequencyDescription = task.Frequency.Description;
            WorkloadID = task.WorkloadID;
            WorkloadDescription = task.Workload.Description;
            TaskTypeID = task.TaskTypeID;
            TaskTypeDescription = task.TaskType.Description;
        }

        public List<TaskDTO> makeList(List<Task> tasks)
        {
            List<TaskDTO> list = new List<TaskDTO>();
            foreach (Task task in tasks)
            {
                TaskDTO taskDTO = new TaskDTO(task);
                list.Add(taskDTO);
            }
            return list;
        }
    }
}
