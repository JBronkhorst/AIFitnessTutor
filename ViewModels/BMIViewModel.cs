using AIFitnessTutor.Models;
using System.ComponentModel.DataAnnotations;

namespace AIFitnessTutor.ViewModels
{
    public class BMIViewModel
    {
        public BMIViewModel()
        {
            BMIItems = new List<BMI>(); // Initialize the list to avoid null reference exceptions
        }

        [Display(Name = "Date")]
        [Required(ErrorMessage = "Date is required")]
        public DateOnly MeasureDate { get; set; }

        [Display(Name = "Height (in cm)")]
        [Required(ErrorMessage = "Height is required")]
        public float? Height { get; set; }

        [Display(Name = "Weight (in kg)")]
        [Required(ErrorMessage = "Weight is required")]
        public float? Weight { get; set; }

        [Display(Name = "Waist Circumference")]
        [Required(ErrorMessage = "Waist circumference is required")]
        public float WaistCircumference { get; set; }

        public float? BMIScore { get; set; }

        public List<BMI> BMIItems { get; set; } // List of BMI items
    }
}
