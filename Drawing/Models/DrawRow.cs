using DrawingService;

namespace Drawing.Models
{
    public record DrawRow : IComparable<DrawRow>
    {
        public int Number { get; set; }

        public Lesson? First { get; init; }

        public Lesson? Second { get; init; }

        public int CompareTo(DrawRow? other)
        {
            return other == null ? -1 : Number.CompareTo(other.Number);
        }
    }
}
