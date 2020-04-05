using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tut3.Models;
using tut3.Services;

namespace tut3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentController : ControllerBase
    {
        public EnrollmentController(IStudentDbService studentsDbService)
        {
            _studentsDbService = studentsDbService;
        }

        private readonly IStudentDbService _studentsDbService;

        [HttpPost]
        public IActionResult Enroll(Student student)
        {
            EnrollmentData enrollment = _studentsDbService.Enroll(student);

            if (enrollment is null) return BadRequest();
            else return StatusCode(201, enrollment);
        }

        [HttpPost][Route("promotions")]
        public IActionResult Promote(PromotionData data)
        {
            Enrollment enrollment = _studentsDbService.Promote(data);

            if (enrollment is null) return BadRequest();
            else return StatusCode(201, enrollment);
        }
    }

    
}