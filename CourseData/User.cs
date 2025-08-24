using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseRegisterApp.CourseData;

[Table("User")]
[Index("Email", Name = "IX_User", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(256)]
    public string Email { get; set; } = null!;

    [StringLength(256)]
    public string? FirstName { get; set; }

    [StringLength(256)]
    public string? LastName { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<CourseUser> CourseUsers { get; set; } = new List<CourseUser>();

    [InverseProperty("LecturerNavigation")]
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
