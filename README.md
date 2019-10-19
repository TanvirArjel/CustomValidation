# AspNetCore.CustomValidation
 This is a cusotm model validaton library for ASP.NET Core projects.
 
 ## How do I get started?
 
 Configuring **TanvirArjel.CustomValidation** into your ASP.NET Core project is as simple as below:
 
 1. First install the `AspNetCore.CustomValidation` nuget package into your project as follows:
 
    `Install-Package AspNetCore.CustomValidation`
    
 2. Then decorate your class propeties with appropriate Custom validation attributes as follows:
 
        public class Employee
        {
            public string EmployeeId { get; set; }

            public string Name { get; set; }

            [MaxAge(30,10,0)] // 30 Year 10 Months 0 Days
            [MinAge(10,10,0)] // 10 Year 10 Months 0 Days
            public DateTime DateOfBirth { get; set; }

            [MinDate(2019,1,1)] // 2019 January 1
            [MaxDate(2019,10,1)] // 2019 October 1
            public DateTime JoiningDate { get; set; }
            public int FirstNumber { get; set; }

            [GreaterThan(nameof(FirstNumber))]
            public int SecondNumber { get; set; }

            [File(new FileType[]{FileType.Jpg, FileType.Jpeg}, MaxSize = 1024)]
            public IFormFile Photo { get; set; }
        }
        
  ## Whats new in Version 1.0.0?
  
  In version 1.0.0 `AspNetCore.Validation` as follwing validation attributes
  
  **1. FileAttribute**
       To validate file type, file max size, file min size
       
  **2. MaxAgeAttribute**
       To validate maximum age against date of birth value of `DateTime` type
       
  **3. MinAgeAttribute**
       To validate minimum required age against a date of birth value of `DateTime` type.
       
  **4. MaxDateAttribute**
       To set max value validation for a `DateTime` field.
       
  **5. MinDateAttribute**
       To set min value validation for a `DateTime` field.
       
  **6. GreaterThanAttribute**
       To check whether the field value is greater than another field value of same type.
    
  **7. SmallerThanAttribute**
       To check whether the field value is smaller than another field value of same type.
       
  **8. TinyMceRequiredAttribute**
       To enforce required valiaton attribute on the online text editors like TinyMCE, CkEditor etc.
       
   # Note
   
   Dont forget to request your desired validaton attribute by submitting an isse.
  
  
