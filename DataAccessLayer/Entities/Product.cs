using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer;

public class Product :BaseEntity
{
    [Key]
    public int ProductId { get; set; }

    public string? Name { get; set; }

    [Column(TypeName = "money")]
    public decimal Price { get; set; }
}
