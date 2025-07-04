using System.ComponentModel.DataAnnotations;

namespace T1_test.Models;

public class Student
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;
    
    public Guid CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;
}