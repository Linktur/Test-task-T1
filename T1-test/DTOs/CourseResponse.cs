namespace T1_test.DTOs;

public class CourseResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<StudentResponse> Students { get; set; } = new();
}

public class StudentResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
}