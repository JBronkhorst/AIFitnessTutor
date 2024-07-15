namespace AIFitnessTutor.Models
{
    public class BMI
    {
        public int Id { get; set; }
        public DateOnly MeasureDate { get; set; }
        public float? Height { get; set; }
        public float? Weight { get; set; }
        public float WaistCircumference { get; set; }
        public float? BMIScore { get; set; }
    }
}
