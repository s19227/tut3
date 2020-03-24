using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace tut3.Models
{
    public class Student
    {
        public Student() {}

        public Student(int id, string firstName, string lastName, string indexNumber)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            IndexNumber = indexNumber;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IndexNumber { get; set; }
    }
}