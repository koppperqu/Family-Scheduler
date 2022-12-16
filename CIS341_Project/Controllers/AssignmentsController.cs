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
    public class AssignmentsController : Controller
    {
        private readonly FamilySchedulerContext _context;

        public AssignmentsController(FamilySchedulerContext context)
        {
            _context = context;
        }

        // GET: Assignments/Create

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["HouseholdMemberName"] = new SelectList(_context.HouseholdMembers, "HouseholdMemberID", "Name");
            ViewData["TaskDescription"] = new SelectList(_context.Tasks, "TaskID", "Description");
            return View();
        }

        // POST: Assignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("AssignmentID,TaskID,HouseholdMemberID,Date,Completed")] AssignmentDTO assignmentDTO)
        {
            if (ModelState.IsValid)
            {
                Assignment assignment = new Assignment
                {
                    Completed = assignmentDTO.Completed,
                    Date = assignmentDTO.Date,
                    HouseholdMemberID = assignmentDTO.HouseholdMemberID,
                    TaskID = assignmentDTO.TaskID
                };
                _context.Add(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AllAssignments));
            }
            ViewData["HouseholdMemberID"] = new SelectList(_context.HouseholdMembers, "HouseholdMemberID", "Name", assignmentDTO.HouseholdMemberID);
            ViewData["TaskID"] = new SelectList(_context.Tasks, "TaskID", "Description", assignmentDTO.TaskID);
            return View(assignmentDTO);
        }

        // GET: Assignments/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Assignments == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments.Include(i => i.Task)
                .Include(i => i.HouseholdMember)
                .Where(i => i.AssignmentID == id)
                .FirstOrDefaultAsync();
            if (assignment == null)
            {
                return NotFound();
            }
            AssignmentDTO assignmentDTO = new AssignmentDTO(assignment);
            ViewData["HouseholdMemberID"] = new SelectList(_context.HouseholdMembers, "HouseholdMemberID", "Name", assignmentDTO.HouseholdMemberID);
            ViewData["TaskID"] = new SelectList(_context.Tasks, "TaskID", "Description", assignmentDTO.TaskID);
            return View(assignmentDTO);
        }

        // POST: Assignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("AssignmentID,TaskID,HouseholdMemberID,Date,Completed")] AssignmentDTO assignmentDTO)
        {
            if (id != assignmentDTO.AssignmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Assignment assignment = new Assignment
                    {
                        AssignmentID = assignmentDTO.AssignmentID,
                        Completed = assignmentDTO.Completed,
                        Date = assignmentDTO.Date,
                        HouseholdMemberID = assignmentDTO.HouseholdMemberID,
                        TaskID = assignmentDTO.TaskID
                    };
                    _context.Update(assignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentExists(assignmentDTO.AssignmentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AllAssignments));
            }
            ViewData["HouseholdMemberID"] = new SelectList(_context.HouseholdMembers, "HouseholdMemberID", "Name", assignmentDTO.HouseholdMemberID);
            ViewData["TaskID"] = new SelectList(_context.Tasks, "TaskID", "Description", assignmentDTO.TaskID);
            return View(assignmentDTO);
        }

        // GET: Assignments/Delete/5
        [Authorize(Roles = "Admin")]
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
            AssignmentDTO assignmentDTO = new AssignmentDTO(assignment);
            return View(assignmentDTO);
        }

        // POST: Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult AllAssignments()
        {
            List<Assignment> allAssignments = _context.Assignments
                .Include("HouseholdMember")
                .Include("Task").ToList();

            List<AssignmentDTO> assignmentDTOs = new List<AssignmentDTO>();


            foreach (Assignment assignment in allAssignments)
            {
                AssignmentDTO assignmentDTO = new AssignmentDTO(assignment);
                assignmentDTOs.Add(assignmentDTO);
            }

            var householdMembersWithAssignments = (from assignment in allAssignments
                                                   select assignment.HouseholdMember.Name)
                                                   .ToList()
                                                   .Distinct()
                                                   .OrderBy(x => x)
                                                   .ToList();
            ViewBag.HouseholdMembersWithAssignments = householdMembersWithAssignments;

            List<AllAssignmentsDTO> allAssignemtnsDTOs = AllAssignmentsDTO.toListOfAllAssignemntsDTOs(assignmentDTOs, householdMembersWithAssignments);


            return View(allAssignemtnsDTOs);
        }
        public IActionResult MyAssignedTasks()
        {
            if (User.Identity != null && User.Identity.Name != null && User.Identity.IsAuthenticated)
            {
                string loggedInUser = User.Identity.Name;


                ViewBag.loggedInHouseHoldmember = loggedInUser;
                List<Assignment> loggedInUsersAssignments = _context.Assignments
                                                        .Include("HouseholdMember")
                                                        .Include("Task")
                                                        .Where(a => a.HouseholdMember.Name == loggedInUser)
                                                        .ToList();

                List<AssignmentDTO> assignmentDTOs = new List<AssignmentDTO>();


                foreach (Assignment assignment in loggedInUsersAssignments)
                {
                    AssignmentDTO assignmentDTO = new AssignmentDTO(assignment);
                    assignmentDTOs.Add(assignmentDTO);
                }

                var householdMembersWithAssignments = (from assignment in loggedInUsersAssignments
                                                       select assignment.HouseholdMember.Name)
                                                       .ToList()
                                                       .Distinct()
                                                       .OrderBy(x => x)
                                                       .ToList();
                ViewBag.HouseholdMembersWithAssignments = householdMembersWithAssignments;

                List<AllAssignmentsDTO> allAssignemtnsDTOs = AllAssignmentsDTO.toListOfAllAssignemntsDTOs(assignmentDTOs, householdMembersWithAssignments);


                return View(allAssignemtnsDTOs);
            }
            else return RedirectToAction(nameof(MyAssignedTasks));
        }

        public async Task<IActionResult> UpdateAssignmentCompletion(int? id)
        {
            if (User.Identity != null && User.Identity.Name != null && User.Identity.IsAuthenticated)
            {
                var assignment = await _context.Assignments.Include("HouseholdMember").Where(a=>a.AssignmentID==id).FirstOrDefaultAsync();
                if (assignment == null)
                {
                    return NotFound();
                }
                if (User.Identity.Name == assignment.HouseholdMember.Name)
                {
                    assignment.Completed = !assignment.Completed;
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(MyAssignedTasks));
                }
                return Unauthorized();
            }
            else return View();
        }
    }
}
