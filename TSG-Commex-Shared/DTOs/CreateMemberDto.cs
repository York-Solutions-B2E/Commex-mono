using System.ComponentModel.DataAnnotations;

namespace TSG_Commex_Shared.DTOs;

public class CreateMemberDto
{
    [Required]
    public string MemberId { get; set; } = string.Empty;
    
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    
    [Required]
    public DateTime DateOfBirth { get; set; }
    
    [Required]
    public DateTime EnrollmentDate { get; set; }
    
    public bool IsActive { get; set; } = true;
}