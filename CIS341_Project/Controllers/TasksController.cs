using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CIS341_Project.Data;
using Task = CIS341_Project.Models.Task;
using CIS341_Project.Models.DTO;
using Microsoft.AspNetCore.Authorization;

namespace CIS341_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    ///<summary>
    ///This is a controller which is used to perform CRUD on the Tasks in the application.
    /// </summary>
    public class TasksController : Controller
    {
        /// <value>
        /// _context is the private field with a reference to the FamilySchedulerContext.
        /// </value>
        private readonly FamilySchedulerContext _context;

        ///<summary>
        ///This is the constructor for the TasksController that inject the FamilySchedulerContext via dependency injection.
        /// </summary>
        /// <param name="context">Parameter for the DbContext to be injected.</param>
        /// <returns>Nothing</returns>
        public TasksController(FamilySchedulerContext context)
        {
            _context = context;
        }

        ///<summary>
        ///This is the action method for the path GET: /Tasks and it gets all Tasks put them in a list of DTO's to be displayed.
        /// </summary>
        /// <returns>Returns IEnumerable<TaskDTO> which contains all the current tasks in the DB.</returns>
        public async Task<IActionResult> Index()
        {
            var tasks = await _context.Tasks.Include(t => t.Frequency).Include(t => t.TaskType).Include(t => t.Workload).ToListAsync();
            TaskDTO taskDTO = new();
            IEnumerable<TaskDTO> tasksDTO = taskDTO.makeList(tasks);
            return View(tasksDTO);
        }

        ///<summary>
        ///This is the action method for the path GET: Tasks/Create it adds Frequencies, TaskTypes, and Workloads to a select list in viewbag
        ///to be displayed in the view.
        /// </summary>
        /// <returns>Returns the associated ViewResult to render Tasks/Create view.</returns>
        public IActionResult Create()
        {
            ViewData["FrequencyDescription"] = new SelectList(_context.Frequencies, "FrequencyID", "Description");
            ViewData["TaskTypeDescription"] = new SelectList(_context.TaskTypes, "TaskTypeID", "Description");
            ViewData["WorkloadDescription"] = new SelectList(_context.Workloads, "WorkloadID", "Description");
            return View();
        }

        ///<summary>
        ///This is the action method for the path POST: Tasks/Create it validates input data and then creates a task if sucessfull or if it
        ///fails validation it will add Frequencies, TaskTypes, and Workloads to a select list in viewbag along with the currently selected 
        ///values to be displayed in the view.
        /// </summary>
        /// <param name="taskDTO">Takes a TaskDTO object that is bound to by the values from the POST request.</param>
        /// <returns>If sucessful returns a redirect to the /Tasks aka Index of Tasks. If failed validation it returns the failed bound taskDTO
        /// to the Create view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskID,Description,FrequencyID,TaskTypeID,WorkloadID")] TaskDTO taskDTO)
        {
            if (ModelState.IsValid)
            {
                Task task = new Task
                {
                    TaskID = taskDTO.TaskID,
                    Description = taskDTO.Description,
                    FrequencyID = taskDTO.FrequencyID,
                    TaskTypeID = taskDTO.TaskTypeID,
                    WorkloadID = taskDTO.WorkloadID
                };
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FrequencyDescription"] = new SelectList(_context.Frequencies, "FrequencyID", "Description", taskDTO.FrequencyID);
            ViewData["TaskTypeDescription"] = new SelectList(_context.TaskTypes, "TaskTypeID", "Description", taskDTO.TaskTypeID);
            ViewData["WorkloadDescription"] = new SelectList(_context.Workloads, "WorkloadID", "Description", taskDTO.WorkloadID);
            return View(taskDTO);
        }

        ///<summary>
        ///This is the action method for the path GET: Tasks/Edit/{id} it adds Frequencies, TaskTypes, and Workloads to a select list in 
        ///viewbag along with the currently selected values for the requested Task to be displayed in the view.
        /// </summary>
        /// <param name="id">the id of a requested task (TaskID)</param>
        /// <returns>Returns NotFound if not a good id. If it is a good ID it returns the associated view with the taskDTO for said Task with that ID</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.Include(t => t.Frequency).Include(t => t.TaskType).Include(t => t.Workload).FirstAsync(l => l.TaskID == id);
            if (task == null)
            {
                return NotFound();
            }
            TaskDTO taskDTO = new(task);
            ViewData["FrequencyDescription"] = new SelectList(_context.Frequencies, "FrequencyID", "Description", taskDTO.FrequencyDescription);
            ViewData["TaskTypeDescription"] = new SelectList(_context.TaskTypes, "TaskTypeID", "Description", taskDTO.TaskTypeDescription);
            ViewData["WorkloadDescription"] = new SelectList(_context.Workloads, "WorkloadID", "Description", taskDTO.WorkloadDescription);
            return View(taskDTO);
        }

        ///<summary>
        ///This is the action method for the path POST: Tasks/Edit/{id} it edits the selected task with the values from the post.
        /// </summary>
        /// <param name="id">the id of a requested task (TaskID)</param>
        /// <param name="taskDTO">Takes a TaskDTO object that is bound to by the values from the POST request.</param>
        /// <returns>Returns NotFound if not a good id. If the values for POST request are not valid it returns the TaskDTO and add Frequencies, 
        /// TaskTypes, and Workloads to the view bag. If it is a good ID it returns a redirect for the Tasks Index.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskID,Description,FrequencyID,TaskTypeID,WorkloadID")] TaskDTO taskDTO)
        {
            if (id != taskDTO.TaskID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (TaskExists(id))
                    {
                        var task = await _context.Tasks.FirstAsync(l => l.TaskID == id);
                        task.FrequencyID = taskDTO.FrequencyID;
                        task.Description = taskDTO.Description;
                        task.TaskTypeID = taskDTO.TaskTypeID;
                        task.WorkloadID = taskDTO.WorkloadID;
                        await _context.SaveChangesAsync();
                    }
                    else return NotFound();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(taskDTO.TaskID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FrequencyDescription"] = new SelectList(_context.Frequencies, "FrequencyID", "Description", taskDTO.FrequencyDescription);
            ViewData["TaskTypeDescription"] = new SelectList(_context.TaskTypes, "TaskTypeID", "Description", taskDTO.TaskTypeDescription);
            ViewData["WorkloadDescription"] = new SelectList(_context.Workloads, "WorkloadID", "Description", taskDTO.WorkloadDescription);
            return View(taskDTO);
        }

        ///<summary>
        ///This is the action method for the path GET: Tasks/Delete/{id} checks if the id is valid and if so it confirms with the user if they do want to delete the selected Task.
        /// </summary>
        /// <param name="id">The id of a requested task to be deleted(TaskID).</param>
        /// <returns>Returns NotFound if not a good id. If it is a good ID it returns the associated view with the taskDTO for said Task with that ID.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Frequency)
                .Include(t => t.TaskType)
                .Include(t => t.Workload)
                .FirstOrDefaultAsync(m => m.TaskID == id);
            if (task == null)
            {
                return NotFound();
            }
            TaskDTO tDTO = new(task);
            return View(tDTO);
        }

        ///<summary>
        ///This is the action method for the path POST: Tasks/Delete/{id}, this will delete the requested Task from the DB
        /// </summary>
        /// <param name="id">the id of a requested task to be deleted(TaskID).</param>
        /// <param name="task">A bound TaskDTO with the requested TaskID bound</param>
        /// <returns>Returns a problem if there is a issue with the DB, otherwise it will redirect to the Tasks Index after deletion.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, [Bind("TaskID")] TaskDTO task)
        {
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'FamilySchedulerContext.Tasks'  is null.");
            }
            var t = await _context.Tasks.FindAsync(task.TaskID);
            if (t != null)
            {
                _context.Tasks.Remove(t);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskID == id);
        }
    }
}
