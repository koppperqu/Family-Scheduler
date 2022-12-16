using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CIS341_Project.Models.DTO
{
    //Each DTO should be 1 row in the table
    public class AllAssignmentsDTO
    {
        //Date with only date is mm/dd/yyyy (Will be first column in the row)

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        //List of assignments lists, each list represents a household member these will be in alphabetical order
        public List<List<AssignmentDTO>>? HouseholdMembersAssignments { get; set; }

        //This function takes in the full list of assignments and list of all household members with assignemtns and
        //returns a list of DTO's which will be processed in the AllAssignmen
        public static List<AllAssignmentsDTO> toListOfAllAssignemntsDTOs(List<AssignmentDTO> listOfAssignments, List<string> householdMembersNamesWithAssignments)
        {
            //This takes the list of assignments and sorts and groups them by date,
            //then sorts and groups those groups by household member
            var listOfAssignmentsGroupedByDateThenByMember =
                (from assignment in listOfAssignments
                 group assignment by assignment.Date into groupedByDate
                 from groupedByMember in (
                 from assignmentgroupedByDate in groupedByDate
                 group assignmentgroupedByDate by assignmentgroupedByDate.HouseholdMemberName).OrderBy(n => n.Key)
                 group groupedByMember by groupedByDate.Key).OrderBy(d => d.Key);

            List<AllAssignmentsDTO> allAssignmentsDTOs = new List<AllAssignmentsDTO>();

            //First layer is the assignemtns grouped by dates
            foreach (var groupedByDate in listOfAssignmentsGroupedByDateThenByMember)
            {
                AllAssignmentsDTO aADTO = new AllAssignmentsDTO();
                //Set the duedate in the new DTO
                aADTO.DueDate = groupedByDate.Key;
                List<List<AssignmentDTO>>? listOfHouseholdMembersListAssignments = new List<List<AssignmentDTO>>();

                //Makes the group into a list to be able to iterate through in a irregular fasion
                //this allows empty lists to be added for household members who do not have a task on a given date
                var groupedByMember = groupedByDate.ToList();
                //Need to keep track of what item is being processed
                int groupedByMemberIndex = 0;

                for (int i = 0; i < householdMembersNamesWithAssignments.Count();)
                {
                    //If the current groupedMember at postion [groupedByMemberIndex] is not the same as the one at 
                    //householdMembersNamesWithAssignments[i] we need to add a blank list of assignmetns then go to the next one
                    //untill we are at the same one then we add the tasks for that member
                    while (i < householdMembersNamesWithAssignments.Count() && groupedByMemberIndex < groupedByMember.Count() && householdMembersNamesWithAssignments[i] != groupedByMember[groupedByMemberIndex].Key)
                    {
                        List<AssignmentDTO> emptyHouseholdMembersAssignments = new List<AssignmentDTO>();
                        i++;
                        listOfHouseholdMembersListAssignments.Add(emptyHouseholdMembersAssignments);
                    }
                    //Broke out of loop because household member matched and has assignments 

                    //For each assignment they have add it to this list
                    List<AssignmentDTO> householdMembersAssignments = new List<AssignmentDTO>();

                    if (groupedByMemberIndex < groupedByMember.Count())
                    {
                        //add each assignment to the list
                        foreach (var groupedByMemberElement in groupedByMember[groupedByMemberIndex])
                        {
                            householdMembersAssignments.Add(groupedByMemberElement);
                        }
                        groupedByMemberIndex++;
                    }
                    i++;
                    //add to list of householdmemeberAssignments
                    listOfHouseholdMembersListAssignments.Add(householdMembersAssignments);
                }
                aADTO.HouseholdMembersAssignments = listOfHouseholdMembersListAssignments;
                //Add the DTO to the list of DTO's
                allAssignmentsDTOs.Add(aADTO);
            }
            return allAssignmentsDTOs;
        }
    }
}
