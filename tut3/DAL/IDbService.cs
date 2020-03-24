using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using tut3.Models;

namespace tut3.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();
        public void CreateStudent(Student student);
    }
}
