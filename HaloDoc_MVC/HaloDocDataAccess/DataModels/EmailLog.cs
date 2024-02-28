using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaloDocDataAccess.DataModels;

[Table("EmailLog")]
public partial class EmailLog
{
    [Key]
    [Precision(9, 0)]
    public decimal EmailLogId { get; set; }

    [StringLength(500)]
    public string EmailTemplate { get; set; } = null!;

    [StringLength(200)]
    public string SubjectName { get; set; } = null!;

    [Column("EmailID")]
    [StringLength(200)]
    public string EmailId { get; set; } = null!;

    [StringLength(200)]
    public string? ConfirmationNumber { get; set; }

    [StringLength(200)]
    public string? FilePath { get; set; }

    public int? RoleId { get; set; }

    public int? RequestId { get; set; }

    public int? AdminId { get; set; }

    public int? PhysicianId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreateDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? SentDate { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsEmailSent { get; set; }

    public int? SentTries { get; set; }

    public int? Action { get; set; }

    [ForeignKey("AdminId")]
    [InverseProperty("EmailLogs")]
    public virtual Admin? Admin { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("EmailLogs")]
    public virtual Physician? Physician { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("EmailLogs")]
    public virtual Request? Request { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("EmailLogs")]
    public virtual Role? Role { get; set; }
}
