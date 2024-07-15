using System.ComponentModel.DataAnnotations;

namespace AIFitnessTutor.Data.Enum
{
    public enum NutritionRestriction
    {
        [Display(Name = "Vegan")]
        Vegan,
        [Display(Name = "Dairy free")]
        DairyFree,
        [Display(Name = "Keto")]
        Keto,
        [Display(Name = "Gluten free")]
        GlutenFree,
        [Display(Name = "Paleo")]
        Paleo,
        [Display(Name = "Raw foodism")]
        RawFoodism,
        [Display(Name = "Diabetes")]
        Diabetes,
        [Display(Name = "Kosher")]
        Kosher,
        [Display(Name = "Halal")]
        Halal,
        [Display(Name = "Pesecatrian")]
        Pescetarian
    }
}
