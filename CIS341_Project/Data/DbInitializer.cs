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

namespace CIS341_Project.Data
{
    public static class DbInitializer
    {
        private readonly static string AdminName = "admin@admin.com";
        private readonly static List<string> Names = new List<string> {"jimbo@jimbo.com", "tommy@tommy.com", "beth@beth.com" };
        public static void Initialize(FamilySchedulerContext familySchedulerContext)
        {
            familySchedulerContext.Database.EnsureCreated();

            if (!familySchedulerContext.Assignments.Any())
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
                var frequency = new Frequency
                {
                    Description = "Once a week",
                    Value = 1
                };
                familySchedulerContext.Frequencies.Add(frequency);
                familySchedulerContext.SaveChanges();

                frequency = new Frequency
                {
                    Description = "Four times a week",
                    Value = 4
                };
                familySchedulerContext.Frequencies.Add(frequency);
                familySchedulerContext.SaveChanges();

                //Add workloads
                var workload = new Workload
                {
                    Description = "Hard",
                    Value = 8
                };
                familySchedulerContext.Workloads.Add(workload);
                familySchedulerContext.SaveChanges();

                workload = new Workload
                {
                    Description = "Easy",
                    Value = 2
                };
                familySchedulerContext.Workloads.Add(workload);
                familySchedulerContext.SaveChanges();

                workload = new Workload
                {
                    Description = "Extremely Hard",
                    Value = 10
                };
                familySchedulerContext.Workloads.Add(workload);
                familySchedulerContext.SaveChanges();

                //Add task types
                var taskType = new TaskType
                {
                    Description = "Yard Work"
                };
                familySchedulerContext.TaskTypes.Add(taskType);
                familySchedulerContext.SaveChanges();

                taskType = new TaskType
                {
                    Description = "Contsruction"
                };
                familySchedulerContext.TaskTypes.Add(taskType);
                familySchedulerContext.SaveChanges();

                taskType = new TaskType
                {
                    Description = "Kitchen Chores"
                };
                familySchedulerContext.TaskTypes.Add(taskType);
                familySchedulerContext.SaveChanges();

                //Add tasks
                var task = new Task
                {
                    Description = "Clean Dishes",
                    FrequencyID = 2,
                    TaskTypeID = 3,
                    WorkloadID = 2
                };
                familySchedulerContext.Tasks.Add(task);
                familySchedulerContext.SaveChanges();

                task = new Task
                {
                    Description = "Take out trash",
                    FrequencyID = 2,
                    TaskTypeID = 3,
                    WorkloadID = 2
                };
                familySchedulerContext.Tasks.Add(task);
                familySchedulerContext.SaveChanges();

                task = new Task
                {
                    Description = "Build a tree house",
                    FrequencyID = 1,
                    TaskTypeID = 2,
                    WorkloadID = 3
                };
                familySchedulerContext.Tasks.Add(task);
                familySchedulerContext.SaveChanges();

                task = new Task
                {
                    Description = "Rake Leaves",
                    FrequencyID = 1,
                    TaskTypeID = 1,
                    WorkloadID = 1
                };
                familySchedulerContext.Tasks.Add(task);
                familySchedulerContext.SaveChanges();

                DateTime monday = new DateTime(2022, 11, 6);
                //Add assignments
                //build treehouse 1 time 4th day of week
                var assignment = new Assignment
                {
                    TaskID = 3,
                    HouseholdMemberID = 3,
                    Date = monday.Date.AddDays(3),
                    Completed = false
                };
                familySchedulerContext.Assignments.Add(assignment);
                familySchedulerContext.SaveChanges();
                //rake 1 time
                assignment = new Assignment
                {
                    TaskID = 4,
                    HouseholdMemberID = 1,
                    Date = monday.Date,
                    Completed = true
                };
                familySchedulerContext.Assignments.Add(assignment);
                familySchedulerContext.SaveChanges();
                //take out trash 4 times
                assignment = new Assignment
                {
                    TaskID = 2,
                    HouseholdMemberID = 1,
                    Date = monday.Date,
                    Completed = true
                };
                familySchedulerContext.Assignments.Add(assignment);
                familySchedulerContext.SaveChanges();

                assignment = new Assignment
                {
                    TaskID = 2,
                    HouseholdMemberID = 2,
                    Date = monday.Date.AddDays(1),
                    Completed = false
                };
                familySchedulerContext.Assignments.Add(assignment);
                familySchedulerContext.SaveChanges();

                assignment = new Assignment
                {
                    TaskID = 2,
                    HouseholdMemberID = 1,
                    Date = monday.Date.AddDays(3),
                    Completed = false
                };
                familySchedulerContext.Assignments.Add(assignment);
                familySchedulerContext.SaveChanges();

                assignment = new Assignment
                {
                    TaskID = 2,
                    HouseholdMemberID = 3,
                    Date = monday.Date.AddDays(6),
                    Completed = false
                };
                familySchedulerContext.Assignments.Add(assignment);
                familySchedulerContext.SaveChanges();
                //Clean Dishes 4 times
                assignment = new Assignment
                {
                    TaskID = 1,
                    HouseholdMemberID = 3,
                    Date = monday.Date.AddDays(2),
                    Completed = false
                };
                familySchedulerContext.Assignments.Add(assignment);
                familySchedulerContext.SaveChanges();

                assignment = new Assignment
                {
                    TaskID = 1,
                    HouseholdMemberID = 1,
                    Date = monday.Date,
                    Completed = true
                };
                familySchedulerContext.Assignments.Add(assignment);
                familySchedulerContext.SaveChanges();

                assignment = new Assignment
                {
                    TaskID = 1,
                    HouseholdMemberID = 2,
                    Date = monday.Date.AddDays(4),
                    Completed = false
                };
                familySchedulerContext.Assignments.Add(assignment);
                familySchedulerContext.SaveChanges();

                assignment = new Assignment
                {
                    TaskID = 1,
                    HouseholdMemberID = 3,
                    Date = monday.Date.AddDays(7),
                    Completed = false
                };
                familySchedulerContext.Assignments.Add(assignment);
                familySchedulerContext.SaveChanges();
            }

        }
    }
}