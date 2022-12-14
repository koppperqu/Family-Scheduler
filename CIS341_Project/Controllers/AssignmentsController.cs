using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CIS341_Project.Data;
using CIS341_Project.Models;
using CIS341_Project.Models.DTO;
using Microsoft.AspNetCore.Authorization;

namespace CIS341_Project.Controllers
{
    [Authorize]
    public class AssignmentsController : Controller
    {
        private readonly FamilySchedulerContext _context;

        public AssignmentsController(FamilySchedulerContext context)
        {
            _context = context;
        }

        // GET: Assignments/Create
        public IActionResult Create()
        {
            ViewData["HouseholdMemberID"] = new SelectList(_context.HouseholdMembers, "HouseholdMemberID", "Name");
            ViewData["TaskID"] = new SelectList(_context.Tasks, "TaskID", "TaskID");
            return View();
        }

        // POST: Assignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Create([Bind("AssignmentID,TaskID,HouseholdMemberID,Date,Completed")] Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HouseholdMemberID"] = new SelectList(_context.HouseholdMembers, "HouseholdMemberID", "Name", assignment.HouseholdMemberID);
            ViewData["TaskID"] = new SelectList(_context.Tasks, "TaskID", "TaskID", assignment.TaskID);
            return View(assignment);
        }

        // GET: Assignments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Assignments == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            ViewData["HouseholdMemberID"] = new SelectList(_context.HouseholdMembers, "HouseholdMemberID", "Name", assignment.HouseholdMemberID);
            ViewData["TaskID"] = new SelectList(_context.Tasks, "TaskID", "TaskID", assignment.TaskID);
            return View(assignment);
        }

        // POST: Assignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssignmentID,TaskID,HouseholdMemberID,Date,Completed")] Assignment assignment)
        {
            if (id != assignment.AssignmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentExists(assignment.AssignmentID))
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
            ViewData["HouseholdMemberID"] = new SelectList(_context.HouseholdMembers, "HouseholdMemberID", "Name", assignment.HouseholdMemberID);
            ViewData["TaskID"] = new SelectList(_context.Tasks, "TaskID", "TaskID", assignment.TaskID);
            return View(assignment);
        }

        // GET: Assignments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Assignments == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments
                .Include(a => a.HouseholdMember)
                .Include(a => a.Task)
                .FirstOrDefaultAsync(m => m.AssignmentID == id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // POST: Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Assignments == null)
            {
                return Problem("Entity set 'FamilySchedulerContext.Assignments'  is null.");
            }
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment != null)
            {
                _context.Assignments.Remove(assignment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllAssignments));
        }

        private bool AssignmentExists(int id)
        {
          return _context.Assignments.Any(e => e.AssignmentID == id);
        }
        public IActionResult AllAssignments()
        {
            List<Assignment> allAssignments = _context.Assignments.Include("HouseholdMember").Include("Task").ToList();

            List<AssignmentDTO> assignmentDTOs = new List<AssignmentDTO>();


            foreach (Assignment assignment in allAssignments)
            {
                AssignmentDTO assignmentDTO = new AssignmentDTO(assignment);
                assignmentDTOs.Add(assignmentDTO);
            }

                var householdMembersWithAssignments = (from assignment in allAssignments
                                                   select assignment.HouseholdMember.Name).ToList().Distinct().OrderBy(x => x).ToList();
            ViewBag.HouseholdMembersWithAssignments = householdMembersWithAssignments;

            List<AllAssignmentsDTO> allAssignemtnsDTOs = AllAssignmentsDTO.toListOfAllAssignemntsDTOs(assignmentDTOs, householdMembersWithAssignments);


            return View(allAssignemtnsDTOs);
        }

        public IActionResult MyAssignedTasks()
        {
            //For not app will only displays beths tasks/assignments since identity is not fully implemented yet
            //In the furture grab logged in user and display their assignments
            string loggedInUser = "Jimbo";
            ViewBag.loggedInHouseHoldmember = loggedInUser;
            List <Assignment> loggedInUsersAssignments = _context.Assignments.Include("HouseholdMember").Include("Task").Where(a=>a.HouseholdMember.Name==loggedInUser).ToList();

            List<AssignmentDTO> assignmentDTOs = new List<AssignmentDTO>();


            foreach (Assignment assignment in loggedInUsersAssignments)
            {
                AssignmentDTO assignmentDTO = new AssignmentDTO(assignment);
                assignmentDTOs.Add(assignmentDTO);
            }

            var householdMembersWithAssignments = (from assignment in loggedInUsersAssignments
                                                   select assignment.HouseholdMember.Name).ToList().Distinct().OrderBy(x => x).ToList();
            ViewBag.HouseholdMembersWithAssignments = householdMembersWithAssignments;

            List<AllAssignmentsDTO> allAssignemtnsDTOs = AllAssignmentsDTO.toListOfAllAssignemntsDTOs(assignmentDTOs, householdMembersWithAssignments);


            return View(allAssignemtnsDTOs);
        }
    }
}
