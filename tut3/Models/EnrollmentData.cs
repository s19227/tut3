using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tut3.Models
{
    public class EnrollmentData
    {
        public EnrollmentData() { }
        
        public EnrollmentData(string firstName, string lastName, string studies, string semester)
        {
            FirstName = firstName;
            LastName = lastName;
            Studies = studies;
            Semester = semester;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Studies { get; set; }
        public string Semester { get; set; }
    }
}
