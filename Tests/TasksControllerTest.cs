using CIS341_Project.Controllers;
using CIS341_Project.Data;
using CIS341_Project.Models;
using CIS341_Project.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Tests
{
    //Naming test method
    //The name of the method being tested.
    //The scenario under which it's being tested.
    //The expected behavior when the scenario is invoked.
    public class TasksControllerTest
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<FamilySchedulerContext> _contextOptions;
        public TasksControllerTest()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            // These options will be used by the context instances in this test suite, including the connection opened above.
            _contextOptions = new DbContextOptionsBuilder<FamilySchedulerContext>()
                .UseSqlite(_connection)
                .Options;

            // Create the schema and seed some data
            using var context = new FamilySchedulerContext(_contextOptions);

            //Initalizes DB with content
            if (context.Database.EnsureCreated())
            {
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
                var task = new CIS341_Project.Models.Task
                {
                    Description = "Clean Dishes",
                    FrequencyID = 2,
                    TaskTypeID = 3,
                    WorkloadID = 2
                };
                context.Tasks.Add(task);
                context.SaveChanges();

                task = new CIS341_Project.Models.Task
                {
                    Description = "Take out trash",
                    FrequencyID = 2,
                    TaskTypeID = 3,
                    WorkloadID = 2
                };
                context.Tasks.Add(task);
                context.SaveChanges();

                task = new CIS341_Project.Models.Task
                {
                    Description = "Build a tree house",
                    FrequencyID = 1,
                    TaskTypeID = 2,
                    WorkloadID = 3
                };
                context.Tasks.Add(task);
                context.SaveChanges();

                task = new CIS341_Project.Models.Task
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

        // DRY-ify the creation of the DbContext.
        FamilySchedulerContext CreateContext() => new(_contextOptions);

        // Close database connection.
        public void Dispose() => _connection?.Dispose();

        [Fact]
        public void Index_ReturnsViewResultWithFourTasks()
        {
            // Arrange
            FamilySchedulerContext context = CreateContext();
            var controller = new TasksController(context);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result.Result); // Do we return a ViewResult?
            // Note that Index() returns a Task -- we can observe the result by using the Result value.
            var model = Assert.IsType<List<TaskDTO>>(viewResult.Model); // Does the model contain the correct type
            Assert.Equal(4, model.Count); // Does the list have the right amount of widgets?
        }
        [Fact]
        public void CreateGet_ReturnsAView()
        {
            //Arrange
            FamilySchedulerContext context = CreateContext();
            var controller = new TasksController(context);
            //Act
            var result = controller.Create();
            //Assert
            Assert.IsType<ViewResult>(result);
        }
        [Fact]  public void CreatePost_GivenValidInput_RedirectToActionIndex()
        {
            //Arrange
            FamilySchedulerContext context = CreateContext();
            var controller = new TasksController(context);
            var expectedContent = new TaskDTO() { 
                TaskID = 5,
                Description = "New Task",
                FrequencyDescription = "Hard",
                TaskTypeDescription = "Yard Work",
                WorkloadDescription = "Once a week"
            };
            //Act
            var result = controller.Create(expectedContent);
            //Assert
            Assert.IsType<RedirectToActionResult>(result);
        }
        [Fact] public void CreatePost_GivenInvalidInput_ReturnViewWithTaskDTO()
        {
            //Arrange
            FamilySchedulerContext context = CreateContext();
            var controller = new TasksController(context);
            controller.ModelState.AddModelError("error", "there's an error");
            //Act
            var result = controller.Create(new TaskDTO());
            //Assert
            var viewResult = Assert.IsType<ViewResult>(result.Result);
            Assert.IsType<TaskDTO>(viewResult.Model);
        }

        [Fact]
        public void EditGet_NoTaskIDSpecified_ReturnNotFound()
        {
            //Arrange
            FamilySchedulerContext context = CreateContext();
            var controller = new TasksController(context);

            //Act
            var result = controller.Edit(null);

            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void EditGet_NoTasksTable_ReturnNotFound()
        {
            //Arrange
            FamilySchedulerContext context = CreateContext();
            var controller = new TasksController(context);
            //Unsure how to drop a table to test but that would go here

            //Act
            var result = controller.Edit(0);

            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Fact]
        public void EditGet_NoTaskForGivenID_ReturnNotFound()
        {
            //Arrange
            FamilySchedulerContext context = CreateContext();
            var controller = new TasksController(context);

            //Act
            var result = controller.Edit(100);

            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Fact]
        public void EditGet_ValidTaskID_ReturnTaskDTO()
        {
            //Arrange
            FamilySchedulerContext context = CreateContext();
            var controller = new TasksController(context);

            //Act
            var result = controller.Edit(1);
            //Assert
            var viewResult = Assert.IsType<ViewResult>(result.Result);
            Assert.IsType<TaskDTO>(viewResult.Model);
        }

        [Fact] public void EditPost_idDoesNotMatchTaskID_ReturnNotFound() { }
        [Fact] public void EditPost_TaskForGivenTaskIDDoesNotExist_ReturnNotFound() { }
        [Fact] public void EditPost_ValidInput_RedirectToActionIndex() { }
        [Fact] public void EditPost_InvalidInput_ReturnViewWithTaskDTO() { }
        [Fact] public void DeleteGet_NoTaskIDGiven_ReturnNotFound() { }
        [Fact] public void DeleteGet_NoTaskTable_ReturnNotFound() { }
        [Fact] public void DeleteGet_ValidTaskIDGiven_ReturnViewWithTaskDTO() { }
        [Fact] public void DeleteGet_GivenTaskIDDoesNotExist_ReturnNotFound() { }
        [Fact] public void DeletePost_NoTaskTable_ReturnProblem() { }
        [Fact] public void DeletePost_ValidDataGiven_RedirectToActionIndex() { }

        //Not sure if the tests below are supposed to be included but since there is code
        //that relies on them in the controller I would assume so
        [Fact] public void TaskExists_GivenATaskThatExists_ReturnTrue() { }
        [Fact] public void TaskExists_GivenATaskThatDoesNotExist_ReturnFalse() { }
    }
}