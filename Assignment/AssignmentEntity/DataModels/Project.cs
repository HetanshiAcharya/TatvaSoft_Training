using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AssignmentEntity.DataModels;

[Table("Project")]
public partial class Project
{
    [Key]
    public int Id { get; set; }

    [StringLength(128)]
    public string ProjectName { get; set; } = null!;

    [StringLength(128)]
    public string? Assignee { get; set; }

    public int? DomainId { get; set; }

    [StringLength(256)]
    public string? Description { get; set; }

    [StringLength(128)]
    public string? Domain { get; set; }

    [StringLength(128)]
    public string? City { get; set; }

    public TimeOnly? DueDate { get; set; }

    [ForeignKey("DomainId")]
    [InverseProperty("Projects")]
    public virtual Domain? DomainNavigation { get; set; }
}
