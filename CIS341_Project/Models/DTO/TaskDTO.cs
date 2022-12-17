using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CIS341_Project.Models.DTO
{
    public class TaskDTO
    {
        public int TaskID { get; set; }
        [DisplayName("Task")]
        [Required]
        [StringLength(100)]

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
        //This method takes a list of tasks and returns them as a list of taskDTO
        //To be able to be passed to the view
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
