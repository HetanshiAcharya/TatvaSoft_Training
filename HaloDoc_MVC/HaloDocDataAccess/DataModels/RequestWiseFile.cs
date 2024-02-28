using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaloDocDataAccess.DataModels;

[Table("RequestWiseFile")]
public partial class RequestWiseFile
{
    [Key]
    public int RequestWiseFileId { get; set; }

    public int RequestId { get; set; }

    [StringLength(500)]
    public string FileName { get; set; } = null!;

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    public int PhysicianId { get; set; }

    public int AdminId { get; set; }

    public short? DocType { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsFrontSide { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsCompensation { get; set; }

    [Column("IP")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsFinalize { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsDeleted { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsPatientRecords { get; set; }
}
