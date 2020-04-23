using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using tut3.Encription;
using tut3.Models;
using tut3.Services;

namespace tut3.DAL
{
    public class StudentDbService : IStudentDbService
    {
        private const string CONNECTION_STRING = "Data Source=db-mssql;Initial Catalog=s19227;Integrated Security=True";

        public EnrollmentData Enroll(Student student)
        {
            EnrollmentData enrollment = new EnrollmentData();

            /* Check if all the required data has been delivered */
            if (
                student.IndexNumber is null ||
                student.IndexNumber.Equals(string.Empty) ||
                student.FirstName is null ||
                student.FirstName.Equals(string.Empty) ||
                student.LastName is null ||
                student.LastName.Equals(string.Empty) ||
                student.BirthDate is null ||
                student.BirthDate.Equals(string.Empty) ||
                student.Studies is null ||
                student.Studies.Equals(string.Empty)
            ) { return null; }

            /* Connect to the database */
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    connection.Open();
                    var transaction = connection.BeginTransaction();
                    command.Transaction = transaction;

                    /* Check if provided studies exists */
                    command.CommandText = "SELECT * FROM Studies WHERE Name = @Name";
                    command.Parameters.AddWithValue("@Name", student.Studies);
                    var reader = command.ExecuteReader();
                    if (!reader.Read())
                    {
                        reader.Close();
                        transaction.Rollback();
                        return null;
                    }
                    string idStudies = reader["IdStudy"].ToString();
                    reader.Close();

                    /* Check if index number is not occupied */
                    command.CommandText = "SELECT * FROM Student WHERE IndexNumber = @IndexNumber";
                    command.Parameters.AddWithValue("@IndexNumber", student.IndexNumber);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        reader.Close();
                        transaction.Rollback();
                        return null;
                    }
                    reader.Close();

                    /* Check if enrollment for this studies already exists */
                    command.CommandText = "SELECT * FROM Enrollment WHERE Semester = 1 AND IdStudy = @IdStudy ORDER BY StartDate";
                    command.Parameters.AddWithValue("@IdStudy", idStudies);
                    reader = command.ExecuteReader();
                    if (!reader.Read())
                    {
                        reader.Close();

                        /* Add a new enrollment */
                        command.CommandText = "SELECT MAX(IdEnrollment) FROM Enrollment";
                        reader = command.ExecuteReader();

                        int idEnrollment = -1;
                        if (!reader.Read()) idEnrollment = 1;
                        else idEnrollment = (int)reader["IdEnrollment"];
                        idEnrollment++;
                        reader.Close();

                        command.CommandText = "INSERT INTO Enrollment VALUES (@IdEnrollment, 1, @IdStudy, @StartDate)";
                        command.Parameters.AddWithValue("@IdEnrollment", idEnrollment);
                        command.Parameters.AddWithValue("@IdStudy", idStudies);
                        command.Parameters.AddWithValue("@StartDate", DateTime.Now.ToString("yyyy-MM-dd"));

                        reader = command.ExecuteReader();

                        command.Parameters.AddWithValue("@IdEnrollment", idEnrollment);
                        reader.Close();
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@IdEnrollment", reader["IdEnrollment"]);
                        reader.Close();
                    }

                    command.CommandText = "INSERT INTO Student VALUES (@IndexNumber, @FirstName, @LastName, @BirthDate, @IdEnrollment)";
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@BirthDate", student.BirthDate);

                    reader = command.ExecuteReader();
                    reader.Close();
                    transaction.Commit();

                    enrollment.FirstName = student.FirstName;
                    enrollment.LastName = student.LastName;
                    enrollment.Semester = 1.ToString();
                    enrollment.Studies = student.Studies;
                }
            }

            return enrollment;
        }

        public Enrollment Promote(PromotionData data)
        {
            Enrollment enrollment = new Enrollment();

            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    connection.Open();

                    /* Check if enrollment with studies and semester provided exists */
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "SELECT * FROM Enrollment e " +
                                          "JOIN Studies s ON s.IdStudy = e.IdStudy " +
                                          "WHERE e.Semester = @Semester AND s.Name = @Name";
                    command.Parameters.AddWithValue("@Semester", data.Semester);
                    command.Parameters.AddWithValue("@Name", data.Name);

                    var reader = command.ExecuteReader();
                    if (!reader.Read())
                    {
                        reader.Close();
                        return null;
                    }
                    reader.Close();

                    /* Execute procedure */
                    command.CommandText = "PromoteStudents";
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.Clear();

                    command.Parameters.AddWithValue("@StudyName", data.Name);
                    command.Parameters.AddWithValue("@Semester", data.Semester);
                    command.ExecuteNonQuery();

                    /* Get new enrollment object */
                    command.Parameters.Clear();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "SELECT Semester, StartDate, s.Name as Name FROM Enrollment e " +
                                          "JOIN Studies s ON s.IdStudy = e.IdStudy " +
                                          "WHERE e.Semester = @Semester AND s.Name = @Name";

                    Console.WriteLine((Convert.ToInt32(data.Semester) + 1).ToString());

                    command.Parameters.AddWithValue("@Semester", (Convert.ToInt32(data.Semester) + 1).ToString());
                    command.Parameters.AddWithValue("@Name", data.Name);


                    reader = command.ExecuteReader();
                    reader.Read();

                    enrollment.Studies = reader["Name"].ToString();
                    enrollment.Semester = reader["Semester"].ToString();
                    enrollment.StartDate = reader["StartDate"].ToString();

                    reader.Close();
                }
            }

            return enrollment;
        }

        public List<EnrollmentData> GetEnrollmentDataByStudentID(string index)
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

            return enrollments;
        }

        public List<Student> GetStudents()
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

            return students;
        }

        public LoginCheckResult CheckLogin(LoginRequest loginRequest)
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    connection.Open();

                    /* Check if given index number exists */
                    command.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber = @IndexNumber";
                    command.Parameters.AddWithValue("@IndexNumber", loginRequest.Login);

                    var reader = command.ExecuteReader();
                    if (!reader.Read())
                    {
                        reader.Close();
                        return null;
                    }
                    reader.Close();

                    /* Get salt */
                    command.CommandText = "SELECT salt FROM Student WHERE IndexNumber = @IndexNumber";
                    reader = command.ExecuteReader();
                    reader.Read();
                    string salt = reader["salt"].ToString();
                    reader.Close();

                    /* Get password */
                    command.CommandText = "SELECT s.pass " +
                                          "FROM Student s " +
                                          "WHERE IndexNumber = @IndexNumber; ";
                    reader = command.ExecuteReader();
                    reader.Read();
                    string password = reader["pass"].ToString();
                    reader.Close();

                    /* Validate password */
                    if (!Salt.Validate(loginRequest.Password, salt, password))
                    {
                        return null;
                    }

                    /* Get the roles of the logged in user */
                    var roles = new List<string>();
                    command.CommandText = "SELECT r.roleName " +
                                          "FROM Student s " +
                                          "JOIN Role_Student sr ON s.IndexNumber = sr.studentId " +
                                          "JOIN Role r ON sr.roleId = r.idRole " +
                                          "WHERE IndexNumber = @IndexNumber AND pass = @pass;";
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        roles.Add(reader["roleName"].ToString());
                    }
                    reader.Close();

                    /* Create refresh token */
                    string refreshToken = Guid.NewGuid().ToString();
                    command.CommandText = "UPDATE Student " +
                                          "Set refresh = @refresh " +
                                          "WHERE IndexNumber = @IndexNumber;";
                    command.Parameters.AddWithValue("@refresh", refreshToken);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        refreshToken = reader["refresh"].ToString();
                    }
                    reader.Close();

                    return new LoginCheckResult(roles, refreshToken);
                }
            }
        }

        public string CheckRefreshToken(RefreshTokenRequest tokenRequest)
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    connection.Open();

                    /* Check if given index number exists */
                    command.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber = @IndexNumber";
                    command.Parameters.AddWithValue("@IndexNumber", tokenRequest.Login);

                    var reader = command.ExecuteReader();
                    if (!reader.Read())
                    {
                        reader.Close();
                        return null;
                    }
                    reader.Close();

                    /* Check token */
                    command.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber = @IndexNumber AND refresh = @refresh";
                    command.Parameters.AddWithValue("@refresh", tokenRequest.RefreshToken);

                    reader = command.ExecuteReader();
                    if (!reader.Read())
                    {
                        reader.Close();
                        return null;
                    }
                    reader.Close();

                    /* Create new token */
                    string refreshToken = Guid.NewGuid().ToString();
                    command.CommandText = "UPDATE Student " +
                                          "Set refresh = @newrefresh " +
                                          "WHERE IndexNumber = @IndexNumber;";
                    command.Parameters.AddWithValue("@newrefresh", refreshToken);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        refreshToken = reader["refresh"].ToString();
                    }
                    reader.Close();

                    return refreshToken;
                }
            }
        }
    }
}
