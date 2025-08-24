using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseRegisterApp.CourseData;

[Table("Course")]
[Index("CourseName", Name = "NonClusteredIndex-20250823-215620", IsUnique = true)]
public partial class Course
{
    [Key]
    public int CourseId { get; set; }

    [StringLength(256)]
    public string CourseName { get; set; } = null!;

    public DateOnly CourseStart { get; set; }

    public DateOnly CourseEnd { get; set; }

    public int MaxStudents { get; set; }

    public int Lecturer { get; set; }

    [InverseProperty("Course")]
    public virtual ICollection<CourseUser> CourseUsers { get; set; } = new List<CourseUser>();

    [ForeignKey("Lecturer")]
    [InverseProperty("Courses")]
    public virtual User LecturerNavigation { get; set; } = null!;
}
