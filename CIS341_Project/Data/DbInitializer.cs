using CIS341_Project.Models;
using Microsoft.EntityFrameworkCore;
using Task = CIS341_Project.Models.Task;

namespace CIS341_Project.Data
{
    public static class DbInitializer
    {
        public static void Initialize(FamilySchedulerContext context)
        {
            context.Database.EnsureCreated();
            // OR use below if you want to use migrations
            //context.Database.Migrate();

            if (context.Assignments.Any())
            {
                return;
            }
            //Add household members
            var householdMember = new HouseholdMember
            {
                Name = "Jimbo"
            };
            context.HouseholdMembers.Add(householdMember);
            context.SaveChanges();

            householdMember = new HouseholdMember
            {
                Name = "Tommy"
            };
            context.HouseholdMembers.Add(householdMember);
            context.SaveChanges();

            householdMember = new HouseholdMember
            {
                Name = "Beth"
            };
            context.HouseholdMembers.Add(householdMember);
            context.SaveChanges();

            //Add frequencies
            var frequency = new Frequency
            {
                Description = "Once a week",
                Value = 1
            };
            context.Frequencies.Add(frequency);
            context.SaveChanges();

            frequency = new Frequency
            {
                Description = "Four times a week",
                Value = 4
            };
            context.Frequencies.Add(frequency);
            context.SaveChanges();

            //Add workloads
            var workload = new Workload
            {
                Description = "Hard",
                Value = 8
            };
            context.Workloads.Add(workload);
            context.SaveChanges();

            workload = new Workload
            {
                Description = "Easy",
                Value = 2
            };
            context.Workloads.Add(workload);
            context.SaveChanges();

            workload = new Workload
            {
                Description = "Extremely Hard",
                Value = 10
            };
            context.Workloads.Add(workload);
            context.SaveChanges();

            //Add task types
            var taskType = new TaskType
            {
                Description = "Yard Work"
            };
            context.TaskTypes.Add(taskType);
            context.SaveChanges();

            taskType = new TaskType
            {
                Description = "Contsruction"
            };
            context.TaskTypes.Add(taskType);
            context.SaveChanges();

            taskType = new TaskType
            {
                Description = "Kitchen Chores"
            };
            context.TaskTypes.Add(taskType);
            context.SaveChanges();

            //Add tasks
            var task = new Task
            {
                Description = "Clean Dishes",
                FrequencyID = 2,
                TaskTypeID = 3,
                WorkloadID = 2
            };
            context.Tasks.Add(task);
            context.SaveChanges();

            task = new Task
            {
                Description = "Take out trash",
                FrequencyID = 2,
                TaskTypeID = 3,
                WorkloadID = 2
            };
            context.Tasks.Add(task);
            context.SaveChanges();

            task = new Task
            {
                Description = "Build a tree house",
                FrequencyID = 1,
                TaskTypeID = 2,
                WorkloadID = 3
            };
            context.Tasks.Add(task);
            context.SaveChanges();

            task = new Task
            {
                Description = "Rake Leaves",
                FrequencyID = 1,
                TaskTypeID = 1,
                WorkloadID = 1
            };
            context.Tasks.Add(task);
            context.SaveChanges();

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
            context.Assignments.Add(assignment);
            context.SaveChanges();
            //rake 1 time
            assignment = new Assignment
            {
                TaskID = 4,
                HouseholdMemberID = 1,
                Date = monday.Date,
                Completed = true
            };
            context.Assignments.Add(assignment);
            context.SaveChanges();
            //take out trash 4 times
            assignment = new Assignment
            {
                TaskID = 2,
                HouseholdMemberID = 1,
                Date = monday.Date,
                Completed = true
            };
            context.Assignments.Add(assignment);
            context.SaveChanges();

            assignment = new Assignment
            {
                TaskID = 2,
                HouseholdMemberID = 2,
                Date = monday.Date.AddDays(1),
                Completed = false
            };
            context.Assignments.Add(assignment);
            context.SaveChanges();

            assignment = new Assignment
            {
                TaskID = 2,
                HouseholdMemberID = 1,
                Date = monday.Date.AddDays(3),
                Completed = false
            };
            context.Assignments.Add(assignment);
            context.SaveChanges();

            assignment = new Assignment
            {
                TaskID = 2,
                HouseholdMemberID = 3,
                Date = monday.Date.AddDays(6),
                Completed = false
            };
            context.Assignments.Add(assignment);
            context.SaveChanges();
            //Clean Dishes 4 times
            assignment = new Assignment
            {
                TaskID = 1,
                HouseholdMemberID = 3,
                Date = monday.Date.AddDays(2),
                Completed = false
            };
            context.Assignments.Add(assignment);
            context.SaveChanges();

            assignment = new Assignment
            {
                TaskID = 1,
                HouseholdMemberID = 1,
                Date = monday.Date,
                Completed = true
            };
            context.Assignments.Add(assignment);
            context.SaveChanges();

            assignment = new Assignment
            {
                TaskID = 1,
                HouseholdMemberID = 2,
                Date = monday.Date.AddDays(4),
                Completed = false
            };
            context.Assignments.Add(assignment);
            context.SaveChanges();

            assignment = new Assignment
            {
                TaskID = 1,
                HouseholdMemberID = 3,
                Date = monday.Date.AddDays(7),
                Completed = false
            };
            context.Assignments.Add(assignment);
            context.SaveChanges();
        }
    }
}