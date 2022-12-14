using System.ComponentModel;

namespace CIS341_Project.Models.DTO
{
    public class TaskDTO
    {
        public int TaskID { get; set; }
        [DisplayName("Task")]
        public string Description { get; set; }
        [DisplayName("Frequency")]
        public string FrequencyDescription { get; set; }
        [DisplayName("Workload")]
        public string WorkloadDescription { get; set; }
        [DisplayName("Task Type")]
        public string TaskTypeDescription { get; set; }

        public TaskDTO() { }
        public TaskDTO(Task task)
        {
            TaskID = task.TaskID;
            Description = task.Description;
            FrequencyDescription = task.Frequency.Description;
            WorkloadDescription = task.Workload.Description;
            TaskTypeDescription = task.TaskType.Description;
        }

        internal List<TaskDTO> makeList(List<Task> tasks)
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
