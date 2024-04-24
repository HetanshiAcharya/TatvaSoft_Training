using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entity.DataModels;

[Table("Student")]
public partial class Student
{
    [Key]
    public int Id { get; set; }

    [StringLength(128)]
    public string Name { get; set; } = null!;

    [StringLength(128)]
    public string Department { get; set; } = null!;

    public int? Semester { get; set; }

    public int? Age { get; set; }

    public int? Fees { get; set; }
   

    [StringLength(500)]
    public string? PhotoId { get; set; }
}
