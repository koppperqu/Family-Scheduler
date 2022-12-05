using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CIS341_Project.Models
{
    public class HouseholdMember
    {
        public int HouseholdMemberID { get; set; } //PK
        [Required]
        [StringLength(50)]
        [DisplayName("Househould Member's Name")]
        public string Name { get; set; } = string.Empty;

        // Navigation properties
        [DisplayName("Assignments By Household Member")]
        public ICollection<Assignment> AssignmentsByHouseholdMember { get; set; } = null!;
    }
}
