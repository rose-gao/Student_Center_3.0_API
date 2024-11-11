﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_Center_3._0_Database.Models
{
    public class StudentCourseHistory
    {
        [Key, ForeignKey("User")]
        [Column(Order = 0)]
        public int userNum { get; set; }

        // not a FK to course; student may have courses not currently offered by the school (ex. high school credits)
        [Key]
        [Column(Order = 1)]
        public string course {  get; set; }


    }
}
