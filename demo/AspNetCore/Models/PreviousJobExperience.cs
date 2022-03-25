using System;
using System.ComponentModel.DataAnnotations;
using TanvirArjel.CustomValidation.Attributes;

namespace AspNetCore.Models
{
    public class PreviousJobExperience
    {
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [CompareTo(nameof(StartDate), ComparisonType.GreaterThan)]
        public DateTime? EndDate { get; set; }

        [RequiredIf(nameof(StartDate), ComparisonType.NotEqual, null)]
        public string JobName { get; set; }
    }
}
