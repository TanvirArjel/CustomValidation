using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AspNetCore.CustomValidation.Attributes;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.CustomValidation.Demo.Models
{
    public class Employee : IValidatableObject
    {
        [DisplayName("Name")]
        [FixedLength(5, ErrorMessage = "{0} should be exactly {1} characters long.")]
        [TextEditorRequired]
        [Required]
        public string Name { get; set; }

        [MinAge(1, 0, 0, ErrorMessage = "Min age should be 1 year.")]
        [DataType(DataType.Date)]
        [DisplayName("Date Of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [MinDate(2019, 1, 1, ErrorMessage = "{0} should be minimun 2019 January 1.")] // 2019 January 1
        [MaxDate(2019, 10, 1, ErrorMessage = "{0} cannot be greater than {1}.")] // 2019 October 1
        [CompareTo(nameof(DateOfBirth), ComparisonType.GreaterThan)]
        [DisplayName("Joining Date")]
        public DateTime? JoiningDate { get; set; }

        [Display(Name = "First Number")]
        public int FirstNumber { get; set; }

        [CompareTo(nameof(FirstNumber), ComparisonType.GreaterThanOrEqual)]
        [Display(Name = "Second Number")]
        public int? SecondNumber { get; set; }

        ////[FileType(new FileType[] { FileType.Mp4, FileType.Mp3 }, ErrorMessage = "{0} should be in {1} formats.")]
        ////[FileMinSize(10000, ErrorMessage = "{0} should be at least {1}.")]
        [File(FileType.Avi, MinSize = 1024)]
        public IFormFile Photo { get; set; }

        // public TimeSpan EntryTime { get; set; }

        // [CompareTo(nameof(EntryTime), ComparisonType.GreaterThan)]
        // public TimeSpan OutTime { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            // FileOptions fileOptions = new FileOptions()
            // {
            //    FileTypes = new FileType[] {FileType.Jpeg,FileType.Jpg},
            //    MinSize = 124,
            //    MaxSize = Convert.ToInt32(AppSettings.GetValue("DemoSettings:MaxFileSize"))
            // };

            // ValidationResult fileValidationResult = validationContext.ValidateFile(nameof(Photo), fileOptions);
            // validationResults.Add(fileValidationResult);

            // ValidationResult minAgeValidationResult = validationContext.ValidateMinAge(nameof(DateOfBirth), 10, 0, 0);
            // validationResults.Add(minAgeValidationResult);
            return validationResults;
        }
    }
}
