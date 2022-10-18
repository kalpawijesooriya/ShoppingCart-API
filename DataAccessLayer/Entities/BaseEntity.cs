

using MessagePack;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer;

public abstract class BaseEntity
{
    [Column(Order = 0)]
    public Guid Id { get; set; }=Guid.NewGuid();
    public int Status { get; set; } = 1;
    public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdateDate { get; set; } 
}
