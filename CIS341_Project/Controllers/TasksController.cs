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
    [Authorize]
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
            var familySchedulerContext = _context.Tasks.Include(t => t.Frequency).Include(t => t.TaskType).Include(t => t.Workload);
            TaskDTO taskDTO = new();
            IEnumerable<TaskDTO> tasks = taskDTO.makeList(await familySchedulerContext.ToListAsync());
            return View(tasks);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            ViewData["FrequencyDescription"] = new SelectList(_context.Frequencies, "Description", "FrequencyID");
            ViewData["TaskTypeDescription"] = new SelectList(_context.TaskTypes, "Description", "TaskTypeID");
            ViewData["WorkloadDescription"] = new SelectList(_context.Workloads, "Description", "WorkloadID");
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskID,Description,FrequencyDescription,TaskTypeDescription,WorkloadDescription")] TaskDTO task)
        {
            if (ModelState.IsValid)
            {
                Task t = new Task();
                t.Description = task.Description;
                t.FrequencyID = _context.Frequencies.First(l => l.Description == task.FrequencyDescription).FrequencyID;
                t.TaskTypeID = _context.TaskTypes.First(l => l.Description == task.TaskTypeDescription).TaskTypeID;
                t.WorkloadID = _context.Workloads.First(l => l.Description == task.WorkloadDescription).WorkloadID;
                _context.Add(t);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FrequencyDescription"] = new SelectList(_context.Frequencies, "Description", "Description", task.FrequencyDescription);
            ViewData["TaskTypeDescription"] = new SelectList(_context.TaskTypes, "Description", "Description", task.TaskTypeDescription);
            ViewData["WorkloadDescription"] = new SelectList(_context.Workloads, "Description", "Description", task.WorkloadDescription);
            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.Include(t => t.Frequency).Include(t => t.TaskType).Include(t => t.Workload).FirstAsync(l=>l.TaskID == id);
            if (task == null)
            {
                return NotFound();
            }
            ViewData["FrequencyDescription"] = new SelectList(_context.Frequencies, "Description", "Description");
            ViewData["TaskTypeDescription"] = new SelectList(_context.TaskTypes, "Description", "Description");
            ViewData["WorkloadDescription"] = new SelectList(_context.Workloads, "Description", "Description");
            TaskDTO tDTO = new(task);
            return View(tDTO);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskID,Description,FrequencyDescription,TaskTypeDescription,WorkloadDescription")] TaskDTO task)
        {
            //Took this out of edit parameter-> 
            //This is giving errors id is not being set when submitted
                if (id != task.TaskID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Task t = new();
                    t.TaskID = task.TaskID;
                    t.Description = task.Description;
                    t.FrequencyID = _context.Frequencies.First(l => l.Description == task.FrequencyDescription).FrequencyID;
                    t.TaskTypeID = _context.TaskTypes.First(l => l.Description == task.TaskTypeDescription).TaskTypeID;
                    t.WorkloadID = _context.Workloads.First(l => l.Description == task.WorkloadDescription).WorkloadID;
                    _context.Update(t);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.TaskID))
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
            ViewData["FrequencyDescription"] = new SelectList(_context.Frequencies, "Description", "Description", task.FrequencyDescription);
            ViewData["TaskTypeDescription"] = new SelectList(_context.TaskTypes, "Description", "Description", task.TaskTypeDescription);
            ViewData["WorkloadDescription"] = new SelectList(_context.Workloads, "Description", "Description", task.WorkloadDescription);
            return View(task);
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
