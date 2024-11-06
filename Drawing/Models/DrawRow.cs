using DrawingService;

namespace Drawing.Models
{
    public record DrawRow : IComparable<DrawRow>
    {
        public int Number { get; set; }

        public bool IsDivided { get; set; }

        public Lesson? FirstLesson { get; init; }

        public Lesson? SecondLesson { get; init; }

        public int CompareTo(DrawRow? other)
        {
            return other == null ? -1 : Number.CompareTo(other.Number);
        }

        public bool IsEmpty => FirstLesson == null && SecondLesson == null;
    }
}
