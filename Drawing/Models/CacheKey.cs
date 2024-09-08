using DrawingService;
using Google.Protobuf.Collections;

namespace Drawing.Models
{
    public class CacheKey<T>
    {
        public Style Style { get; set; }
        public RepeatedField<T> Lessons { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is not CacheKey<T> other)
                return false;

            // Сравниваем стиль
            if (!EqualityComparer<Style>.Default.Equals(Style, other.Style))
                return false;

            // Сравниваем RepeatedField уроков
            return Lessons.Count == other.Lessons.Count && Lessons.SequenceEqual(other.Lessons);
        }

        public override int GetHashCode()
        {
            // Используем HashCode.Combine для создания хэш-кода
            int hash = HashCode.Combine(Style);
            foreach (var lesson in Lessons)
            {
                hash ^= lesson.GetHashCode();
            }
            return hash;
        }
    }
}
