using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TekitouCRUD.Shared.Entities;

public class User
{
    [Key]
    [Column("Userid")]
    public int UserId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string UserName { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
    public string Address { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string CellNumber { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string EmailId { get; set; } = null!;
}
