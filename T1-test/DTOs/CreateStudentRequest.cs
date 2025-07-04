using System.ComponentModel.DataAnnotations;

namespace T1_test.DTOs;

public class CreateStudentRequest
{
    [Required]
    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;
}