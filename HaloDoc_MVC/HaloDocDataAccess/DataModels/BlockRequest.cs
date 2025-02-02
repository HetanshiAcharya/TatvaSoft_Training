﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaloDocDataAccess.DataModels;

public partial class BlockRequest
{
    [Key]
    public int BlockRequestId { get; set; }

    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    [StringLength(50)]
    public string? Email { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsActive { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }

    public int RequestId { get; set; }

    [Column("IP")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("BlockRequests")]
    public virtual Request Request { get; set; } = null!;
}
