using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using tut3.Models;
using tut3.DAL;

namespace tut3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentController : ControllerBase
    {
        private readonly IDbService m_dbService;

        public StudentController(IDbService dbService)
        {
            m_dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {

            return Ok(m_dbService.GetStudents());
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";

            m_dbService.CreateStudent(student);

            return Ok(m_dbService.GetStudents());
        }

        [HttpPut]
        public IActionResult UpdateStudent(int id)
        {
            return Ok("Update completed.");
        }

        [HttpDelete]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Delete completed.");
        }
    }
}