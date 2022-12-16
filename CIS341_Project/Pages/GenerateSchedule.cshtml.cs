using CIS341_Project.Data;
using CIS341_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using TaskSchedulerLibrary;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace CIS341_Project
{
    [Authorize(Roles = "Admin")]
    public class GenerateScheduleModel : PageModel
    {
        [Required]
        [DisplayName("Starting Date")]
        [BindProperty]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now;
        [Required]
        [DisplayName("Maximum Tasks Per Member")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        [BindProperty]
        public int MaxTasksPerUser { get; set; } = 1;
        [BindNever]
        private List<int>? _userIDs { get; set; }
        [BindNever]
        private List<(int TaskID, int Frequency, int Workload)>? _tasks { get; set; }
        private FamilySchedulerContext _context { get; set; }

        public GenerateScheduleModel(FamilySchedulerContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
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
                    list.Add(new(taskInfo.taskID, taskInfo.Frequency, taskInfo.Workload));
                }
                _tasks = list;

                var schedule = TaskSchedulerLibrary.TaskScheduler.ScheduleWeek(StartDate, _userIDs, _tasks, MaxTasksPerUser);
                //Remove all the old assignments
                var allAssignments = await _context.Assignments.ToListAsync();
                foreach (Models.Assignment assignment in allAssignments)
                {
                    _context.Assignments.Remove(assignment);
                }
                await _context.SaveChangesAsync();

                //Make new assigments with returned data

                foreach (TaskSchedulerLibrary.Assignment assignmentScheduled in schedule)
                {
                    Models.Assignment assignment = new Models.Assignment { Date = DateTime.Parse(assignmentScheduled.Date.ToString()), HouseholdMemberID = assignmentScheduled.UserID, TaskID = assignmentScheduled.TaskID };
                    await _context.Assignments.AddAsync(assignment);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("AllAssignments","Assignments");
            }
            return RedirectToPage("GenerateSchedule");
        }
    }
}
