# ASP.NET Core Custom Validation

 This is a custom model validation library for ASP.NET Core projects.
 
## Whats new in version 2.0.0?

   1. This release added localization support for error messages.
   2. FileTypeAttribute, FileMaxSizeAttribute and FileMinSizeAttribute are newly added ValidationAttributes in this release.
   3. TinyMceRequiredAttribute has been renamed to TextEditorRequiredAttribute.
   4. This release also includes some imporant bug fixes.
 
## How do I get started?
 
 Configuring **TanvirArjel.CustomValidation** into your ASP.NET Core project is simple as below:
 
 1. First install the lastest version of `AspNetCore.CustomValidation` [nuget package](https://www.nuget.org/packages/AspNetCore.CustomValidation) into your project as follows:
 
    `Install-Package AspNetCore.CustomValidation`
    
 2. Then decorate your class properties with appropriate Custom validation attributes as follows:
 
        pulic class Employee
        {
             [File(FileType.Jpg, MaxSize = 1024)]
             public IFormFile Photo { get; set; }
        }
        
  ## Client Side validation:
  
  To enable client client side validation for **ASP.NET Core MVC or Razor Pages**:
  
  1. First in the `ConfirugeServices` method of the `Startup` class:
  
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddAspNetCoreCustomValidation();
    }
   
  2. Then please add the latest version of `aspnetcore-custom-validation.min.js` file as follows:
  
    @section Scripts {
      @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
      <script type="text/javascript" src="~/lib/aspnetcore-custom-validation/aspnetcore-custom-validation.min.js"</script>
    }
    
    Or
    
    <script src="~/lib/jquery-validation/dist/jquery.validate.min.js"></script>
    <script src="~/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js"></script>
    <script type="text/javascript" src="~/lib/aspnetcore-custom-validation/aspnetcore-custom-validation.min.js"</script>
    
You can download the `aspnetcore-custom-validation.min.js` from here [aspnetcore-custom-validation-npm](https://www.npmjs.com/package/aspnetcore-custom-validation)

Or using Visusl Studio **Libman** as follows:

    1 ) wwwroot > lib> Add > Client Side Libray

    2. Provider: jsdelivr
       Libray: aspnetcore-custom-validation
    3. Click install
  
        
  ## What contains in version 2.0.0?
  
  In version 2.0.0 `AspNetCore.CustomValidation` contains the following validation attributes:
  
  **1. FileAttribute**
       To validate file type, file max size, file min size etc.
  
  **2. FileTypeAttribute**
       To validate type of a file.
  
  **3. FileMaxSizeAttribute**
       To validate allowed max size of a file.
       
  **4. FileMinSizeAttribute**
       To validate allowed min size of a file.
       
  **5. MaxAgeAttribute**
       To validate maximum age against date of birth value of `DateTime` type.
       
  **6. MinAgeAttribute**
       To validate minimum required age against a date of birth value of `DateTime` type.
       
  **7. MaxDateAttribute**
       To set max value validation for a `DateTime` field.
       
  **8. MinDateAttribute**
       To set min value validation for a `DateTime` field.
       
  **9. TextEditorRequiredAttribute**
       To enforce required valiaton attribute on the online text editors like TinyMCE, CkEditor etc.
       
  **10. CompareToAttribute**
       To compare one property value against another property value of the same object. Comparison types are: Equal, NotEqual, GreaterThan, GreatherThanOrEqual, SmallerThan, SmallerThanOrEqual
       
   # Dynamic Validation
   From version 1.4.0, validation against dynamic values from database, configuration file or any external source added for the following type:
    **1. File Type:** with `ValidateFile()` method
    **1. DateTime Type:** with `ValidateMaxAge()` and `ValidateMinAge()` method as follows:
    
    public class Employee : IValidatableObject
    {
        public DateTime? DateOfBirth { get; set; }
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
    
       
# Note
   
Dont forget to request your desired validation  attribute by submitting an issue.
   
# Request

**If you find this library useful, please don't forget to encouraging me to do such more stuffs by giving a star to this repository. Thank you.**
  
  
