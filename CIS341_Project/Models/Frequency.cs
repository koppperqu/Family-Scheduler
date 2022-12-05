using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CIS341_Project.Models
{
    public class Frequency
    {
        public int FrequencyID { get; set; } //PK
        [Required]
        [StringLength(100)]
        [DisplayName("Frequency Description")]
        public string Description { get; set; } = string.Empty;
        [Required]
        [Range(0, 14)]//0-14 assuming a 1 is 1 time in the week 14 would be 2 times a day
        [DisplayName("Frequency Value")]
        public int Value { get; set; }

        // Navigation properties
        [DisplayName("TasksByFrequency")]
        public ICollection<Task> TasksByFrequency { get; set; } = null!;

    }
}
