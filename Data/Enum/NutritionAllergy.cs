using System.ComponentModel.DataAnnotations;

namespace AIFitnessTutor.Data.Enum
{
    public enum NutritionAllergy
    {
        [Display(Name = "Milk")]
        Milk,
        [Display(Name = "Eggs")]
        Eggs,
        [Display(Name = "Peanuts")]
        Peanuts,
        [Display(Name = "Tree nuts")]
        TreeNuts,
        [Display(Name = "Soy")]
        Soy,
        [Display(Name = "Wheat")]
        Wheat,
        [Display(Name = "Fish")]
        Fish,
        [Display(Name = "Shellfish")]
        Shellfish,
        [Display(Name = "Sesame seeds")]
        SesameSeeds,
        [Display(Name = "Mustard")]
        Mustard,
        [Display(Name = "Celery")]
        Celery,
        [Display(Name = "Lupin")]
        Lupin,
        [Display(Name = "Sulfites")]
        Sulfites,
        [Display(Name = "Mollusks")]
        Mollusks,
        [Display(Name = "Corn")]
        Corn,
        [Display(Name = "Red meat")]
        RedMeat,
        [Display(Name = "Fruits")]
        Fruits,
        [Display(Name = "Vegetables")]
        Vegetables,
        [Display(Name = "Spices")]
        Spices,
        [Display(Name = "Food additives and preservatives")]
        FoodAdditivesAndPreservatives
    }
}
