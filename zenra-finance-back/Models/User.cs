using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("users")] // 👈 match table name exactly
public class User
{
    [Key]
    [Column("id")] // 👈 THIS fixes your error
    public int Id { get; set; }

    [Required]
    [Column("name")]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    [Column("email")]
    public string Email { get; set; }

    [Column("phone")]
    public string? Phone { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("nic")]
    public string? Nic { get; set; }

    [Required]
    [MinLength(6)]
    [Column("password")]
    public string Password { get; set; }

    [Required]
    [Column("profile")]
    public string Profile { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
