using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourseRegisterApp.CourseData;

[Table("CourseUser")]
[Index("CourseId", "UserId", Name = "IX_CourseUser", IsUnique = true)]
public partial class CourseUser
{
    [Key]
    public int CourseUserId { get; set; }

    public int CourseId { get; set; }

    public int UserId { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("CourseUsers")]
    public virtual Course Course { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("CourseUsers")]
    public virtual User User { get; set; } = null!;
}
