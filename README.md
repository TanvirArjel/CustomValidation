## 👑 Custom Validation 👑
This is a custom model validation library for any C# and .NET projects.

 ## ⭐ Give a star ⭐
   
   **If you find this library useful to you, please don't forget to encouraging me to do such more stuffs by giving a star (⭐) to this repository. Thank you.**

## ⚙️ Attributes ⚙️
  
  `TanvirArjel.CustomValidation` contains the following validation attributes:
     
  **1. MaxAgeAttribute**
       To validate maximum age against date of birth value of `DateTime` type.
       
  **2. MinAgeAttribute**
       To validate minimum required age against a date of birth value of `DateTime` type.
       
  **3. MaxDateAttribute**
       To set max value validation for a `DateTime` field.
       
  **4. MinDateAttribute**
       To set min value validation for a `DateTime` field.
       
  **5. TextEditorRequiredAttribute**
       To enforce required valiaton attribute on the online text editors like TinyMCE, CkEditor etc.
       
  **6. CompareToAttribute**
       To compare one property value against another property value of the same object. Comparison types are: Equal, NotEqual, GreaterThan, GreatherThanOrEqual, SmallerThan, SmallerThanOrEqual
       
   **7. RequiredIfAttribute**
       To mark a field required based on the value of another field.
       
  In addition to the above, `TanvirArjel.CustomValidation.AspNetCore` also contains the following validation attributes:
  
  **1. FileAttribute**
       To validate file type, file max size, file min size etc.
  
  **2. FileTypeAttribute**
       To validate type of a file.
  
  **3. FileMaxSizeAttribute**
       To validate allowed max size of a file.
       
  **4. FileMinSizeAttribute**
       To validate allowed min size of a file.
 
## ✈️ How do I get started? ✈️

**For any C# and .NET Application:** First install the lastest version of `TanvirArjel.CustomValidation` [nuget package](https://www.nuget.org/packages/TanvirArjel.CustomValidation/) into your project as follows:
 
    Install-Package TanvirArjel.CustomValidation
    
**For ASP.NET Core Application:** First install the lastest version of `TanvirArjel.CustomValidation.AspNetCore` [nuget package](https://www.nuget.org/packages/TanvirArjel.CustomValidation.AspNetCore/) into your project as follows:
 
    Install-Package TanvirArjel.CustomValidation.AspNetCore
    
**ASP.NET Core Client Side validation:** To enable client client side validation for **ASP.NET Core MVC or Razor Pages**:
  
  1. First in the `ConfirugeServices` method of the `Startup` class:
 
 ```C#
 using TanvirArjel.CustomValidation.AspNetCore.Extensions;

 public static void ConfigureServices(IServiceCollection services)
 {
     services.AddAspNetCoreCustomValidation();
     // or
     services.AddSingleton<IValidationAttributeAdapterProvider, TanvirArjelAttributeAdapterProvider>();
 }
 ```
   
  2. Then please add the latest version of `tanvirarjel.customvalidation.unobtrusive.min.js` file as follows:
 
 ```C#
 @section Scripts {
   @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
   <script type="text/javascript" src="~/lib/tanvirarjel-custom-validation-unobtrusive/tanvirarjel.customvalidation.unobtrusive.min.js"></script>
 }

 Or

 <script src="~/lib/jquery-validation/dist/jquery.validate.min.js"></script>
 <script src="~/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js"></script>
 <script type="text/javascript" src="~/lib/tanvirarjel-custom-validation-unobtrusive/tanvirarjel.customvalidation.unobtrusive.min.js"></script>
 ```
    
You can download the `tanvirarjel.customvalidation.unobtrusive.min.js` from here [tanvirarjel-custom-validation-unobtrusive-npm](https://www.npmjs.com/package/tanvirarjel-custom-validation-unobtrusive)

Or using Visusl Studio **Libman** as follows:

    1. wwwroot > lib> Add > Client Side Libray
    
    2. Provider: jsdelivr
       Libray: tanvirarjel-custom-validation-unobtrusive
    3. Click install     
 
## 🛠️ Usage 🛠️

Then decorate your class properties with appropriate custom validation attributes as follows:

```C#
using TanvirArjel.CustomValidation.Attributes;

pulic class Employee
{
    [Display(Name = "First Number")]
    public int FirstNumber { get; set; }

    [CompareTo(nameof(FirstNumber), ComparisonType.GreaterThanOrEqual)]
    [Display(Name = "Second Number")]
    public int? SecondNumber { get; set; }

    [RequiredIf(nameof(FirstNumber), ComparisonType.Equal, 10)]
    public string ThirdNumber { get; set; }

    [File(FileType.Jpg, MaxSize = 1024)]
    public IFormFile Photo { get; set; }
}
```
        
  
 ## ✔️ Dynamic Validation ✔️
 
   Validation against dynamic values from database, configuration file or any external sources added for the following type:
    **1. File Type:** with `ValidateFile()` method
    **1. DateTime Type:** with `ValidateMaxAge()` and `ValidateMinAge()` method as follows:
 
 ```C#
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
 ```
     
## 🙏 Note 🙏
   
**Dont forget to request your desired validation  attribute by submitting an issue.**
   
## 🐞 Bug Report 🐞

**Dont forget to submit an issue if you face. we will try to resolve as soon as possible.**
