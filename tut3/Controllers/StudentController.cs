using System.Linq;
using Microsoft.AspNetCore.Mvc;
using tut3.Services;

namespace tut3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentController : ControllerBase
    {
        public StudentController(IStudentDbService studentsDbService)
        {
            _studentsDbService = studentsDbService;
        }

        private readonly IStudentDbService _studentsDbService;

        [HttpGet]
        public IActionResult GetStudents()
        {
            var students = _studentsDbService.GetStudents();
            return Ok(students);
        }

        [HttpGet("{@index}")][Route("enrollment")]
        public IActionResult GetEnrollmentDataByStudentID(string index)
        {
            var enrollments = _studentsDbService.GetEnrollmentDataByStudentID(index);             

            if (enrollments.Count > 0) return Ok(enrollments);
            else return NotFound();
        }
    }
}