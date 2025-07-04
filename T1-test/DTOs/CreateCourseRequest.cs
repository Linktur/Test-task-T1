using System.ComponentModel.DataAnnotations;

namespace T1_test.DTOs;

public class CreateCourseRequest
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
}