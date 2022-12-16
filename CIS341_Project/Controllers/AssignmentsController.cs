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
    ///<summary>
    ///This is a controller which is used to perform CRUD on the Assignments in the application and also
    ///let users view their assigned tasks, or admins view all assigned tasks.
    /// </summary>
    public class AssignmentsController : Controller
    {
        /// <value>
        /// _context is the private field with a reference to the FamilySchedulerContext.
        /// </value>
        private readonly FamilySchedulerContext _context;
        ///<summary>
        ///This is the constructor for the AssignmentsController that injects the FamilySchedulerContext via dependency injection.
        /// </summary>
        /// <param name="context">Parameter for the DbContext to be injected.</param>
        /// <returns>Nothing</returns>
        public AssignmentsController(FamilySchedulerContext context)
        {
            _context = context;
        }

        ///<summary>
        ///This is the action method for the path GET: Assignments/Create it adds HouseholdMembers and Tasks to a select list in viewbag
        ///to be displayed in the view.
        /// </summary>
        /// <returns>Returns the associated ViewResult to render Assignments/Create view.</returns>

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["HouseholdMemberName"] = new SelectList(_context.HouseholdMembers, "HouseholdMemberID", "Name");
            ViewData["TaskDescription"] = new SelectList(_context.Tasks, "TaskID", "Description");
            return View();
        }

        ///<summary>
        ///This is the action method for the path POST: Assignments/Create it validates input data and then creates an assignment if sucessfull or if it
        ///fails validation it will add HouseholdMembers and Tasks to a select list in viewbag along with the currently selected 
        ///values to be displayed in the view.
        /// </summary>
        /// <param name="assignmentDTO">Takes a AssignmentDTO object that is bound to by the values from the POST request.</param>
        /// <returns>If sucessful returns a redirect to the /Assignments/AllAssignments. If failed validation it returns the failed bound AssignmentDTO
        /// to the Create view.</returns>
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

        [Authorize(Roles = "Admin")]
        ///<summary>
        ///This is the action method for the path GET: Assignments/Edit/{id} it adds HouseholdMembers and Tasks to a select list in 
        ///viewbag along with the currently selected values for the requested Assignment to be displayed in the view.
        /// </summary>
        /// <param name="id">the id of a requested assignment (AssignmentID)</param>
        /// <returns>Returns NotFound if not a good id. If it is a good ID it returns the associated view with the assignmentDTO for said Assignment with that ID</returns>
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

        ///<summary>
        ///This is the action method for the path POST: Assignments/Edit/{id} it edits the selected Assignments with the values from the post.
        /// </summary>
        /// <param name="id">the id of a requested assignment (AssignmentID)</param>
        /// <param name="assignmentDTO">Takes a AssignmentDTO object that is bound to by the values from the POST request.</param>
        /// <returns>Returns NotFound if not a good id. If the values for POST request are not valid it returns the TaskDTO and adds
        /// HouseholdMembers and Tasks to select list in the view bag. If it is a good ID it returns a redirect for the AllAssignments view.</returns>
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

        ///<summary>
        ///This is the action method for the path GET: Assignments/Delete/{id} checks if the id is valid and if so it confirms with the user if they do want
        ///to delete the selected Assignment.
        /// </summary>
        /// <param name="id">The id of a requested assignment to be deleted(AssignmentID).</param>
        /// <returns>Returns NotFound if not a good id. If it is a good ID it returns the associated view with the assignmentDTO for said Assignment with that ID.</returns>
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

        ///<summary>
        ///This is the action method for the path POST: Assignments/Delete/{id}, this will delete the requested Assignment from the DB
        /// </summary>
        /// <param name="id">the id of a requested assignment to be deleted(AssignmentID).</param>
        /// <returns>Returns a problem if there is a issue with the DB, otherwise it will redirect to the AllAssignments view after deletion.</returns>
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
        private bool AnyAssignments()
        {
            return _context.Assignments.Any();
        }
        private bool AnyAssignmentsForMember(string name)
        {
            return _context.Assignments.Any(a=>a.HouseholdMember.Name==name);
        }


        ///<summary>
        ///This is the action method for the path GET: Assignments/AllAssignments, this will get all assignments from the DB and group/orginize them then 
        ///display them in the view.
        /// </summary>
        /// <returns>Returns an allAssignmentsDTO which contains all assignments grouped by Date and Member. Also returns the associated View for AllAssigments.</returns>
        [Authorize(Roles = "Admin")]
        public IActionResult AllAssignments()
        {
            if (!AnyAssignments())
            {
                ViewBag.NoAssignments = true;
            }
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

        ///<summary>
        ///This is the action method for the path GET: Assignments/MyAssignedTasks, this will get all assignments for the logged in member from the DB and 
        ///group/orginize them then display them in the view.
        /// </summary>
        /// <returns>Returns an allAssignmentsDTO which contains all assignments grouped for a member grouped by date and that member. Also returns the 
        /// associated View for MyAssignedTasks.</returns>
        public IActionResult MyAssignedTasks()
        {

            if (User.Identity != null && User.Identity.Name != null && User.Identity.IsAuthenticated)
            {

                if (!AnyAssignmentsForMember(User.Identity.Name))
                {
                    ViewBag.NoAssignmentsForMember = true;
                }

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

        ///<summary>
        ///This is the action method for the path GET: Assignments/UpdateAssignmentCompletion/{id}, it checks if this assignment is for the logged in user, 
        ///if it is it flips the Completed field, i.e. marking it completed or incompleted.
        /// </summary>
        /// <returns>Returns a redirect to MyAssginedTasks after updated the assignment, or Unauthorized status message if the user is trying to complete a 
        /// assignmetn that is not theirs.</returns>

        public async Task<IActionResult> UpdateAssignmentCompletion(int id)
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
