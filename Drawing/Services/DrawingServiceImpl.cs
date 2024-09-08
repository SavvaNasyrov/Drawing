using Drawing.Models;
using DrawingService;
using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;

namespace Drawing.Services
{
    public class DrawingServiceImpl : Drawer.DrawerBase
    {
        private readonly IMemoryCache _cache;

        private readonly IConfiguration _configuration;

        private static int _classDrawingsCount = 0;

        public DrawingServiceImpl(IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
        }

        public override async Task<DrawResponse> DrawForClass(DrawForClassRequest request, ServerCallContext context)
        {
            if (_cache.TryGetValue(new CacheKey<ClassLesson>() { Style = request.DrawStyle, Lessons = request.Lessons }, out string? cachedPath))
            {
                if (File.Exists(cachedPath))
                {
                    return new DrawResponse() { PathToImage = cachedPath };
                }
                else
                {
                    _cache.Remove(new CacheKey<ClassLesson>() { Style = request.DrawStyle, Lessons = request.Lessons });
                    _classDrawingsCount--;
                }
            }

            var initalizer = request.Lessons.GroupBy(x => x.LessonNumber).Select(x =>
            {
                var arr = x.ToArray();
                if (arr.Length < 2)
                    return new DrawRow() 
                    { 
                        First = new GeneralizedLesson { FirstData = arr[0].Subject, SecondData = arr[0].Teacher, ThirdData = arr[0].Auditory } ,
                        Number = arr[0].LessonNumber,
                    };
                else
                    return new DrawRow()
                    {
                        First = new GeneralizedLesson { FirstData = arr[0].Subject, SecondData = arr[0].Teacher, ThirdData = arr[0].Auditory },
                        Second = new GeneralizedLesson { FirstData = arr[1].Subject, SecondData = arr[1].Teacher, ThirdData = arr[1].Auditory },
                        Number = arr[0].LessonNumber,
                    };
            });
            var set = new SortedSet<DrawRow>(initalizer);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{_configuration["DefaultImagePath"]}class{_classDrawingsCount}.png");

            await Task.Run( () => GeneralDrawer.Draw(set, PhysicalStyle.ToPhisycal(request.DrawStyle), path) );
            _classDrawingsCount++;

            AddToCache(new CacheKey<ClassLesson>() { Style = request.DrawStyle, Lessons = request.Lessons }, path);

            return new DrawResponse() { PathToImage = path };
        }

        public override Task<DrawResponse> DrawForTeacher(DrawForTeacherRequest request, ServerCallContext context)
        {
            return Task.FromResult(new DrawResponse { PathToImage = "null" });
        }

        public override Task<DrawResponse> DrawForAuditory(DrawForAuditoryRequest request, ServerCallContext context)
        {
            return Task.FromResult(new DrawResponse { PathToImage = "null" });
        }

        private T2 AddToCache<T, T2>(CacheKey<T> key, T2 value)
        {
            static void OnEvicition(object key, object? value, EvictionReason reason, object? state)
            {
                if (value is string path)
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch { }
                }
                Console.WriteLine("Evicied");
                _classDrawingsCount--;
            }
            var expiraton = TimeSpan.FromHours(float.Parse(_configuration["SlidingExpirationInHours"]));
            var opts = new MemoryCacheEntryOptions() { SlidingExpiration = expiraton };
            opts.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration() { EvictionCallback = OnEvicition});
            return _cache.Set(key, value, opts);
        }
    }
}
