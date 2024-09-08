namespace Drawing.Models
{
    public record GeneralizedLesson
    {
        public string FirstData { get; init; } = "Нет";

        public string SecondData { get; init; } = "Нет";

        public string ThirdData { get; init; } = "Нет";

        public override string ToString()
        {
            return $"{FirstData}, {SecondData}, {ThirdData}";
        }
    }
}
