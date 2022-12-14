using Microsoft.EntityFrameworkCore;
using CIS341_Project.Models;
using Task = CIS341_Project.Models.Task;

namespace CIS341_Project.Data
{
    public class FamilySchedulerContext : DbContext
    {
        public FamilySchedulerContext(DbContextOptions<FamilySchedulerContext> options) : base(options)
        {
        }

        // Table definitions
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Frequency> Frequencies { get; set; }
        public DbSet<HouseholdMember> HouseholdMembers { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }
        public DbSet<Workload> Workloads { get; set; }
    }
}
