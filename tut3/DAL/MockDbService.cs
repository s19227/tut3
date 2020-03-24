using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using tut3.Models;

namespace tut3.DAL
{
    public class MockDbService : IDbService
    {
        private static List<Student> m_students;

        static MockDbService()
        {
            m_students = new List<Student>
            {
                new Student{ Id = 1, FirstName = "Jan", LastName = "Kowalski" },
                new Student{ Id = 2, FirstName = "Anna", LastName = "Malewski" }
            };
        }

        public void CreateStudent(Student student)
        {
            m_students.Add(student);
        }

        public IEnumerable<Student> GetStudents()
        {
            return m_students;
        }


    }
}
