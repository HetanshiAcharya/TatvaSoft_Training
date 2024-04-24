using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AssignmentEntity.DataModels;

[Table("Domain")]
public partial class Domain
{
    [Key]
    public int Id { get; set; }

    [StringLength(128)]
    public string Name { get; set; } = null!;

    [InverseProperty("DomainNavigation")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
