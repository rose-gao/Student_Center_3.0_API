using System.ComponentModel.DataAnnotations;

namespace Student_Center_3._0_Database.Models
{
    /// <summary>
    /// Table to help group related prerequisites that share a logical relationship
    /// Ex. Course A prerequisites: (Course B AND Course C) OR (1.0 CREDITS from Course D, Course E, Course F)
    /// 
    /// PrereqGroup
    /// groupID     |       groupType       |       creditRequirement
    ///     1       |       AND             |           NULL
    ///     2       |       OR              |           NULL
    ///     3       |       CREDITS         |           1.0
    ///     
    /// CoursePreqrequisites
    /// courseNum   |       prerequisiteNum     |       groupId
    ///     A       |           B               |       1
    ///     A       |           C               |       1
    ///     A       |           NULL            |       2
    ///     A       |           D               |       3
    ///     A       |           E               |       3
    ///     A       |           F               |       3  
    /// </summary>
    public class PrereqGroup
    {
        [Key]
        public int GroupId { get; set; }

        [Required]
        [StringLength(20)]
        public string GroupType { get; set; } // "AND", "OR", "CREDITS"

        public double? CreditRequirement { get; set; } // Only for "CREDITS" type

        // Navigation property: allows access and iteration through all the CoursePrereq
        // records associated with that group
        public ICollection<CoursePrereq> CoursePrereqs { get; set; }
    }
}
