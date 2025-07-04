using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T1_test.Data;
using T1_test.DTOs;
using T1_test.Models;

namespace T1_test.Controllers;

[ApiController]
[Route("[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(ApplicationDbContext context, ILogger<CoursesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Получает коллекцию курсов со студентами
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseResponse>>> GetCourses()
    {
        try
        {
            var courses = await _context.Courses
                .Include(c => c.Students)
                .Select(c => new CourseResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Students = c.Students.Select(s => new StudentResponse
                    {
                        Id = s.Id,
                        FullName = s.FullName
                    }).ToList()
                })
                .ToListAsync();

            return Ok(courses);
        }
        catch (Exception ex)
        {
            _logger.LogError("Something went wrong with courses: " + ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Получает конкретный курс со студентами по ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CourseResponse>> GetCourse(Guid id)
    {
        try
        {
            var course = await _context.Courses
                .Include(c => c.Students)
                .Where(c => c.Id == id)
                .Select(c => new CourseResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Students = c.Students.Select(s => new StudentResponse
                    {
                        Id = s.Id,
                        FullName = s.FullName
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return NotFound($"Course with id {id} not found");
            }

            return Ok(course);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to get course: " + ex);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Создает новый курс
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateCourse([FromBody] CreateCourseRequest request)
    {
        try
        {
            var course = new Course
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourses), new { id = course.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError("Course creation failed " + ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Создает нового студента в курсе с указанным идентификатором
    /// </summary>
    [HttpPost("{id:guid}/students")]
    public async Task<ActionResult> CreateStudent(Guid id, [FromBody] CreateStudentRequest request)
    {
        try
        {
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == id);
            if (!courseExists)
            {
                return NotFound($"Course with id {id} not found");
            }

            var student = new Student
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                CourseId = id
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourses), new { id = student.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError("Student not added " + ex);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Удаляет курс с указанным идентификатором со всеми студентами в нем
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        try
        {
            var course = await _context.Courses
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound($"Course with id {id} not found");
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError("Course deletion failed: " + ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }
} 