using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaloDocDataAccess.DataModels;

[Table("TimeSheetReceipt")]
public partial class TimeSheetReceipt
{
    [Key]
    public int TimeSheetReceiptId { get; set; }

    public int TimeSheetDetailsId { get; set; }

    [Column(TypeName = "character varying")]
    public string? ItemName { get; set; }

    public int? Amount { get; set; }

    [Column(TypeName = "character varying")]
    public string? BillName { get; set; }

    [Column("Bill Extension", TypeName = "character varying")]
    public string? BillExtension { get; set; }

    [Column(TypeName = "character varying")]
    public string? CreatedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column(TypeName = "character varying")]
    public string? ModifiedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }
}
