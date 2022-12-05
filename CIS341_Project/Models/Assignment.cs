using System.ComponentModel;

namespace CIS341_Project.Models
{
    public class Assignment
    {
        public int AssignmentID { get; set; } //PK
        public int TaskID { get; set; } //FK
        public int HouseholdMemberID { get; set; } //FK
        [DisplayName("Date Due")]
        public DateTime Date { get; set; }
        [DisplayName("Assignment Completed?")]
        public bool Completed { get; set; }

        //Navigation properties
        [DisplayName("Task")]
        public Task Task { get; set; } = null!;
        [DisplayName("HouseholdMember")]
        public HouseholdMember HouseholdMember { get; set; } = null!;
    }
}
