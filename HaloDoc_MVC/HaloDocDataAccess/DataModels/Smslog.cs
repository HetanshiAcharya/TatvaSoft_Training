using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaloDocDataAccess.DataModels;

[Table("SMSLog")]
public partial class Smslog
{
    [Key]
    [Column("SMSLogId")]
    public int SmslogId { get; set; }

    [Column("SMSTemplate")]
    [StringLength(500)]
    public string Smstemplate { get; set; } = null!;

    [StringLength(50)]
    public string MobileNumber { get; set; } = null!;

    [StringLength(200)]
    public string? ConfirmationNumber { get; set; }

    public int RoleId { get; set; }

    public int AdminId { get; set; }

    public int RequestId { get; set; }

    public int PhysicianId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreateDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? SentDate { get; set; }

    [Column("IsSMSSent", TypeName = "bit(1)")]
    public BitArray? IsSmssent { get; set; }

    public int SentTries { get; set; }

    public int? Action { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("Smslogs")]
    public virtual Request Request { get; set; } = null!;
}
