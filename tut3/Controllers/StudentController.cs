using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using tut3.Models;
using tut3.DAL;
using System.Data.SqlClient;
using System.Diagnostics;

namespace tut3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentController : ControllerBase
    {
        private const string CONNECTION_STRING = "Data Source=db-mssql;Initial Catalog=s19227;Integrated Security=True";

        [HttpGet]
        public IActionResult GetStudents()
        {
            List<Student> students = new List<Student>();

            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "select s.FirstName, s.LastName, s.BirthDate, st.Name as Studies, e.Semester " +
                                          "from Student s " +
                                          "join Enrollment e on e.IdEnrollment = s.IdEnrollment " +
                                          "join Studies st on st.IdStudy = e.IdStudy; ";

                    connection.Open();

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var student = new Student();

                        student.FirstName = reader["FirstName"].ToString();
                        student.LastName = reader["LastName"].ToString();
                        student.Studies = reader["Studies"].ToString();
                        student.BirthDate = reader["BirthDate"].ToString();
                        student.Semester = reader["Semester"].ToString();

                        students.Add(student);
                    }
                }
            }

            return Ok(students);
        }

        [HttpGet("{@index}")][Route("enrollment")]
        public IActionResult GetEnrollmentDataByStudentID(string index)
        {

            List<EnrollmentData> enrollments = new List<EnrollmentData>();

            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT s.FirstName, s.LastName, st.Name as Studies, e.Semester " +
                                          "FROM Studies st " +
                                          "JOIN Enrollment e on e.IdStudy = st.IdStudy " +
                                          "JOIN Student s on s.IdEnrollment = e.IdEnrollment " +
                                          "WHERE s.IndexNumber = " + index;

                    connection.Open();

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var enrollment = new EnrollmentData();

                        enrollment.FirstName = reader["FirstName"].ToString();
                        enrollment.LastName  = reader["LastName"].ToString();
                        enrollment.Studies   = reader["Studies"].ToString();
                        enrollment.Semester = reader["Semester"].ToString();

                        enrollments.Add(enrollment);
                    }
                }
            }

            if (enrollments.Count > 0) return Ok(enrollments);
            else return NotFound();
        }

        /* Task 3.4: Pass "19999; DROP TABLE Student;" to delete the whole table */

        [HttpGet("{@index}")][Route("cooler-enrollment")]
        public IActionResult GetEnrollmentDataByStudentIDSecure(string index)
        {
            List<EnrollmentData> enrollments = new List<EnrollmentData>();

            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT s.FirstName, s.LastName, st.Name as Studies, e.Semester " +
                                          "FROM Studies st " +
                                          "JOIN Enrollment e on e.IdStudy = st.IdStudy " +
                                          "JOIN Student s on s.IdEnrollment = e.IdEnrollment " +
                                          "WHERE s.IndexNumber = @index";

                    command.Parameters.AddWithValue("@index", index);
                    connection.Open();

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var enrollment = new EnrollmentData();

                        enrollment.FirstName = reader["FirstName"].ToString();
                        enrollment.LastName = reader["LastName"].ToString();
                        enrollment.Studies = reader["Studies"].ToString();
                        enrollment.Semester = reader["Semester"].ToString();

                        enrollments.Add(enrollment);
                    }
                }
            }

            if (enrollments.Count > 0) return Ok(enrollments);
            else return NotFound();
        }

        private class EnrollmentData
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Studies { get; set; }
            public string Semester { get; set; }
        }

        /*[HttpPost]
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
        }*/
        
    }
}