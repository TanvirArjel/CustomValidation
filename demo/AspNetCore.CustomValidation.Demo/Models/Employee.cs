using AspNetCore.CustomValidation.Attributes;
using AspNetCore.CustomValidation.Validators;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspNetCore.CustomValidation.Demo.Models
{
    public class Employee : IValidatableObject
    {
        public string EmployeeId { get; set; }

        public string Name { get; set; }

        //[MaxAge(30,10,0)] // 30 Year 10 Months 0 Days
        //[MinAge(10,10,0)] // 10 Year 10 Months 0 Days
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        //[MinDate(2019,1,1)] // 2019 January 1
        //[MaxDate(2019,10,1)] // 2019 October 1

        [GreaterThan(nameof(DateOfBirth))]
        public DateTime JoiningDate { get; set; }
        public int FirstNumber { get; set; }

        [CompareTo("TEst", ComparisonType.GreaterThan)]
        public int? SecondNumber { get; set; }

        public IFormFile Photo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            FileOptions fileOptions = new FileOptions()
            {
                FileTypes = new FileType[] {FileType.Jpeg,FileType.Jpg},
                MinSize = 124,
                MaxSize = Convert.ToInt32(AppSettings.GetValue("DemoSettings:MaxFileSize"))
            };

            ValidationResult minAgeValidationResult = validationContext.ValidateMinAge(nameof(DateOfBirth), 10, 0, 0);
            validationResults.Add(minAgeValidationResult);
            ValidationResult fileValidationResult = validationContext.ValidateFile(nameof(Photo), fileOptions);
            validationResults.Add(fileValidationResult);
            return validationResults;
        }
    }
}
