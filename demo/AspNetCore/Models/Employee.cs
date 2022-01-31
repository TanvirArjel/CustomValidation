using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AspNetCore.Models;
using Microsoft.AspNetCore.Http;
using TanvirArjel.CustomValidation.AspNetCore.Attributes;
using TanvirArjel.CustomValidation.Attributes;

namespace AspNetCore.CustomValidation.Demo.Models;

public class Employee : IValidatableObject
{
    ////[DisplayName("Name")]
    ////[FixedLength(5, ErrorMessage = "{0} should be exactly {1} characters long.")]
    ////[TextEditorRequired]
    [Required]
    [MinLength(5)]
    [AspNetCore.MyCustomAttributes.FooAtrribute]
    public string FirstName { get; set; }

    [RequiredIf(nameof(FirstName), ComparisonType.Equal, "Tanvir")]
    public string LastName { get; set; }

    [Required]
    [Range(1000, 9999)]
    [Display(Name = "Starting Year")]
    public int? StartingYear { get; set; }

    [Required]
    [Range(1000, 9999)]
    [CompareTo(nameof(StartingYear), ComparisonType.GreaterThanOrEqual, ErrorMessage = "The {0} should be greater than or equal {1}.")]
    [Display(Name = "Ending Year")]
    public int? EndingYear { get; set; }

    ////[DataType(DataType.Date)]
    ////[DisplayName("Date Of Birth")]
    [Required]
    [MinAge(15, 11, 3)]
    [MaxAge(20, 11, 3)]
    public DateTime DateOfBirth { get; set; }

    ////[MinDate(2019, 1, 1, ErrorMessage = "{0} should be minimun 2019 January 1.")] // 2019 January 1
    ////[MaxDate(2019, 10, 1, ErrorMessage = "{0} cannot be greater than {1}.")] // 2019 October 1
    ////[CompareTo(nameof(DateOfBirth), ComparisonType.GreaterThan)]
    [DisplayName("Joining Date")]
    ////[RequiredIf(nameof(DateOfBirth), ComparisonType.Equal, "01-May-2020")]
    public DateTime? JoiningDate { get; set; }

    [Display(Name = "First Number")]
    [RequiredIf(nameof(SecondNumber), ComparisonType.Equal, null)]
    public int? FirstNumber { get; set; }

    ////[RequiredIf(nameof(FirstNumber), ComparisonType.Equal, null)]
    [Display(Name = "Second Number")]
    [CompareTo(nameof(FirstNumber), ComparisonType.GreaterThan)]
    public int? SecondNumber { get; set; }

    ////[FileType(new FileType[] { FileType.Mp4, FileType.Mp3 }, ErrorMessage = "{0} should be in {1} formats.")]
    ////[FileMinSize(10000, ErrorMessage = "{0} should be at least {1}.")]
    [File(FileType.Avi, MinSize = 1024)]
    public IFormFile Photo { get; set; }

    public TimeSpan? EntryTime { get; set; }

    [RequiredIf(nameof(EntryTime), ComparisonType.GreaterThan, "10:00")]
    public TimeSpan? OutTime { get; set; }
    
    public bool IsPhoneRequired { get; set; }

    [RequiredIf(nameof(IsPhoneRequired), ComparisonType.Equal, true)]
    public string Phone { get; set; }

    public PreviousJobExperience[] PreviousJobs { get; set; } = new PreviousJobExperience[2];

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
