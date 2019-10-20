using AspNetCore.CustomValidation.Attributes;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetCore.CustomValidation.Demo.Models
{
    public class Employee
    {
        public string EmployeeId { get; set; }

        [CompareTo(nameof(DateOfBirth), CompareType.Equality)]
        public string Name { get; set; }

        [MaxAge(30,10,0)] // 30 Year 10 Months 0 Days
        [MinAge(10,10,0)] // 10 Year 10 Months 0 Days
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        //[MinDate(2019,1,1)] // 2019 January 1
        //[MaxDate(2019,10,1)] // 2019 October 1
        public DateTime JoiningDate { get; set; }
        public int FirstNumber { get; set; }

        [GreaterThan(nameof(FirstNumber))]
        public int SecondNumber { get; set; }

        [File(new FileType[]{FileType.Jpg, FileType.Jpeg}, MaxSize = 1024)]
        public IFormFile Photo { get; set; }
    }
}
