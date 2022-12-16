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
    public class TasksController : Controller
    {
        private readonly FamilySchedulerContext _context;

        public TasksController(FamilySchedulerContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            var tasks = await _context.Tasks.Include(t => t.Frequency).Include(t => t.TaskType).Include(t => t.Workload).ToListAsync();
            TaskDTO taskDTO = new();
            IEnumerable<TaskDTO> tasksDTO = taskDTO.makeList(tasks);
            return View(tasksDTO);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            ViewData["FrequencyDescription"] = new SelectList(_context.Frequencies, "FrequencyID", "Description");
            ViewData["TaskTypeDescription"] = new SelectList(_context.TaskTypes, "TaskTypeID", "Description");
            ViewData["WorkloadDescription"] = new SelectList(_context.Workloads, "WorkloadID", "Description");
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Tasks/Edit/5
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

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskID,Description,FrequencyID,TaskTypeID,WorkloadID")] TaskDTO taskDTO)
        {
            //Took this out of edit parameter-> 
            //This is giving errors id is not being set when submitted
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

        // GET: Tasks/Delete/5
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

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, [Bind("TaskID")] TaskDTO task)
        {
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'FamilySchedulerContext.Tasks'  is null.");
            }
            var t = await _context.Tasks.FindAsync(task.TaskID);
            if (task != null)
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
