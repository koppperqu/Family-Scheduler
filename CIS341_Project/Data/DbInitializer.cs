using CIS341_Project.Areas.Identity.Data;
using CIS341_Project.Data;
using CIS341_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System;
using System.Xml.Linq;
using Task = CIS341_Project.Models.Task;
using System.Collections.Generic;

namespace CIS341_Project.Data
{
    public static class DbInitializer
    {
        private readonly static string AdminName = "admin@admin.com";
        private readonly static List<string> Names = new List<string> { "jimbo@jimbo.com", "tommy@tommy.com", "beth@beth.com" };
        private readonly static List<(string, int)> Frequencies = new List<(string, int)> {("Everyday of week", 0),("Weekly", 1)};
        private readonly static List<(string, int)> Workloads = new List<(string, int)> { ("Hard", 10), ("Easy", 1), ("Medium", 5) };
        private readonly static List<string> TaskTypes = new List<string> { "Yard work", "Construction", "Kitchen Chore", "Cleaning", "Cooking", "Other" };
        private readonly static List<(string, string, string, string)> Tasks = new List<(string, string, string, string)>
        {
            ("Clean Dishes","Everyday of week","Kitchen Chore", "Medium"),
            ("Take out trash","Weekly","Kitchen Chore", "Easy"),
            ("Build a tree house","Weekly","Construction", "Hard"),
            ("Cook dinner","Everyday of week","Cooking", "Medium"),
            ("Cook lunch","Everyday of week","Cooking", "Medium"),
            ("Cook breakfast","Everyday of week","Cooking", "Medium"),
            ("Rake leaves","Weekly","Yard work", "Medium")
        };
        public static void Initialize(FamilySchedulerContext familySchedulerContext)
        {
            familySchedulerContext.Database.EnsureCreated();

            if (!familySchedulerContext.Frequencies.Any())
            {
                //Add household members
                var allNames = Names;
                allNames.Add(AdminName);
                foreach (string name in allNames)
                {
                    var householdMember = new HouseholdMember { Name = name };
                    familySchedulerContext.HouseholdMembers.Add(householdMember);
                    familySchedulerContext.SaveChanges();
                }

                //Add frequencies
                foreach (var tuple in Frequencies)
                {
                    var frequency = new Frequency
                    {
                        Description = tuple.Item1,
                        Value = tuple.Item2
                    };
                    familySchedulerContext.Frequencies.Add(frequency);
                    familySchedulerContext.SaveChanges();
                }

                //Add workloads
                foreach (var tuple in Workloads)
                {
                    var workload = new Workload
                    {
                        Description = tuple.Item1,
                        Value = tuple.Item2
                    };
                    familySchedulerContext.Workloads.Add(workload);
                    familySchedulerContext.SaveChanges();
                }
                //Add task types
                foreach (var taskTypeDescription in TaskTypes)
                {
                    var taskType = new TaskType
                    {
                        Description = taskTypeDescription
                    };
                    familySchedulerContext.TaskTypes.Add(taskType);
                    familySchedulerContext.SaveChanges();
                }
                //Add tasks
                foreach (var tuple in Tasks)
                {
                    var task = new Task
                    {
                        Description = tuple.Item1,
                        FrequencyID = familySchedulerContext.Frequencies.First(x=>x.Description == tuple.Item2).FrequencyID,
                        TaskTypeID = familySchedulerContext.TaskTypes.First(x => x.Description == tuple.Item3).TaskTypeID,
                        WorkloadID = familySchedulerContext.Workloads.First(x => x.Description == tuple.Item4).WorkloadID,
                    };
                    familySchedulerContext.Tasks.Add(task);
                    familySchedulerContext.SaveChanges();
                }

                //Add assignments here this is code copied from the Generate Schedule page
                //I dont think you can call the post method otherwise I would do that
                //I realize now that I could have thrown it into a model class and used a method from
                //that to be able to keep my code DRY

                var _userIDs = (from members in familySchedulerContext.HouseholdMembers select members.HouseholdMemberID).ToList();

                var tasksInfo = (from task in familySchedulerContext.Tasks.Include("Frequency").Include("Workload")
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
                var _tasks = list;

                var schedule = TaskSchedulerLibrary.TaskScheduler.ScheduleWeek(DateTime.Now, _userIDs, _tasks, 10);
                //Remove all the old assignments
                var allAssignments = familySchedulerContext.Assignments.ToList();
                foreach (Assignment assignment in allAssignments)
                {
                    familySchedulerContext.Assignments.Remove(assignment);
                }
                familySchedulerContext.SaveChanges();

                //Make new assigments with returned data
                foreach (TaskSchedulerLibrary.Assignment assignmentScheduled in schedule)
                {
                    Assignment assignment = new Assignment { Date = DateTime.Parse(assignmentScheduled.Date.ToString()), HouseholdMemberID = assignmentScheduled.UserID, TaskID = assignmentScheduled.TaskID };
                    familySchedulerContext.Assignments.Add(assignment);
                }
                familySchedulerContext.SaveChanges();
            }

        }
    }
}