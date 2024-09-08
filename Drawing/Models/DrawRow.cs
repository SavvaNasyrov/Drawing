namespace Drawing.Models
{
    public record DrawRow : IComparable<DrawRow>
    {
        public int Number { get; set; }

        public GeneralizedLesson? First { get; init; }

        public GeneralizedLesson? Second { get; init; }

        public int CompareTo(DrawRow? other)
        {
            return other == null ? -1 : Number.CompareTo(other.Number);
        }
    }
}
