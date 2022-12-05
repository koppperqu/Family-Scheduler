namespace TaskSchedulerLibrary
{
    public class Assignment
    {
        // ID of the user the task is assigned to
        public int UserID { get; set; }
        // ID of the task the assignment applies to
        public int TaskID { get; set; }
        // which date is the assignment for
        public DateOnly Date { get; set; }
    }
}
