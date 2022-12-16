using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CIS341_Project.Models.DTO
{
    public class AssignmentDTO
    {
        public int AssignmentID { get; set; } //PK
        [BindNever]
        [DisplayName("Task Description")]
        public string? TaskDescription { get; set; }
        public int TaskID { get; set; }
        [BindNever]
        [DisplayName("Household Member")]
        public string? HouseholdMemberName { get; set; }
        public int HouseholdMemberID { get; set; }
        [DisplayName("Date Due")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [DisplayName("Assignment Completed?")]
        public bool Completed { get; set; }
        public AssignmentDTO() { }

        public AssignmentDTO(Assignment assignment)
        {
            AssignmentID = assignment.AssignmentID;
            TaskDescription = assignment.Task.Description;
            TaskID = assignment.TaskID;
            HouseholdMemberName = assignment.HouseholdMember.Name;
            HouseholdMemberID = assignment.HouseholdMemberID;
            Date = assignment.Date;
            Completed = assignment.Completed;
        }
    }
}
