using System.ComponentModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CIS341_Project.Models.DTO
{
    public class AssignmentDTO
    {
        public int AssignmentID { get; set; } //PK
        public string TaskDescription { get; set; }
        public string HouseholdMemberName { get; set; }
        [DisplayName("Date Due")]
        public DateOnly Date { get; set; }
        [DisplayName("Assignment Completed?")]
        public bool Completed { get; set; }

        public AssignmentDTO(Assignment assignment)
        {
            AssignmentID = assignment.AssignmentID;
            TaskDescription = assignment.Task.Description;
            HouseholdMemberName = assignment.HouseholdMember.Name;
            Date = DateOnly.FromDateTime(assignment.Date);
            Completed = assignment.Completed;
        }
    }
}
