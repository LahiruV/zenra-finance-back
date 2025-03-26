using System;
using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    public string? Phone { get; set; }  // Nullable and optional

    public string? Address { get; set; }  // Nullable and optional

    public string? Nic { get; set; }  // Nullable and optional

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Profile is required")]
    public string Profile { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
