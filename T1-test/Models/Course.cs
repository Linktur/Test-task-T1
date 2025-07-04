using System.ComponentModel.DataAnnotations;

namespace T1_test.Models;

public class Course
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}