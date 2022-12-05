using CIS341_Project.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CIS341_Project
{
    public class GenerateScheduleModel : PageModel
    {
        [Required]
        [DisplayName("Starting Date")]
        [BindProperty]
        public DateTime StartDate { get; set; } = DateTime.Now;
        [Required]
        [DisplayName("Maximum Tasks Per Member")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        [BindProperty]
        public int MaxTasksPerUser { get; set; } = 1;
        [BindNever]
        private List<int> _userIDs { get; set; }
        [BindNever]
        private List<(int TaskID, int Frequency, int Workload)> _tasks { get; set; }
        private FamilySchedulerContext _context { get; set; }

        public GenerateScheduleModel(FamilySchedulerContext context)
        {
            _context = context;
            _userIDs = (from members in _context.HouseholdMembers select members.HouseholdMemberID).ToList();

            var tasksInfo = (from task in _context.Tasks.Include("Frequency").Include("Workload")
                             select new
                             {
                                 taskID = task.TaskID,
                                 Frequency = task.Frequency.Value,
                                 Workload = task.Workload.Value,
                             });

            List<(int TaskID, int Frequency, int Workload)> list = new();
            foreach (var taskInfo in tasksInfo)
            {
                list.Add(new (taskInfo.taskID, taskInfo.Frequency, taskInfo.Workload));
            }
            _tasks=list;
        }
        public void OnGet()
        {
        }
        public void OnPostAsync()
        {
            var schedule = TaskSchedulerLibrary.TaskScheduler.ScheduleWeek(StartDate, _userIDs, _tasks, MaxTasksPerUser);
            //Make new assigments with returned data
        }
    }
}
