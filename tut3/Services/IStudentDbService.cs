using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tut3.Models;

namespace tut3.Services
{
    public interface IStudentDbService
    {
        List<Student> GetStudents();
        List<EnrollmentData> GetEnrollmentDataByStudentID(string index);

        EnrollmentData Enroll(Student student);
        Enrollment Promote(PromotionData data);
    }
}
