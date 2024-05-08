using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaloDocDataAccess.DataModels;

[Table("TimeSheet")]
public partial class TimeSheet
{
    [Key]
    public int TimeSheetId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime EndDate { get; set; }

    public bool IsFinalized { get; set; }

    public bool IsApproved { get; set; }

    [Column(TypeName = "character varying")]
    public string? AdminDescription { get; set; }

    public int? BonusAmount { get; set; }

    public int? TotalAmount { get; set; }

    public int PhysicianId { get; set; }

    [Column(TypeName = "character varying")]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column(TypeName = "character varying")]
    public string? ModifiedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }
}
