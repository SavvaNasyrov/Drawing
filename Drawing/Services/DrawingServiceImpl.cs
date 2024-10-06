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

        public DrawingServiceImpl(IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
        }

        public override async Task<DrawResponse> Draw(DrawRequest request, ServerCallContext context)
        {
            if (_cache.TryGetValue(request.DrawStyle.ToString() + string.Join("", request.Lessons.Select( x => $"{x.First}{x.Second}{x.Third}")), out string? cachedPath))
            {
                if (File.Exists(cachedPath))
                {
                    return new DrawResponse() { PathToImage = cachedPath };
                }
                else
                {
                    _cache.Remove(new CacheKey<Lesson>() { Style = request.DrawStyle, Lessons = request.Lessons });
                }
            }

            var initalizer = request.Lessons.GroupBy(x => x.LessonNumber).Select(x =>
            {
                var arr = x.ToArray();
                if (arr.Length < 2)
                    return new DrawRow() 
                    { 
                        First = arr[0] ,
                        Number = arr[0].LessonNumber,
                    };
                else
                    return new DrawRow()
                    {
                        First = arr[0],
                        Second = arr[1],
                        Number = arr[0].LessonNumber,
                    };
            });
            var set = new SortedSet<DrawRow>(initalizer);
            string path = $"/images/{Guid.NewGuid().ToString()}.png";
            Console.WriteLine(path);

            await Task.Run( () => GeneralDrawer.Draw(set, PhysicalStyle.ToPhisycal(request.DrawStyle), path) );

            AddToCache(request.DrawStyle.ToString() + string.Join("", request.Lessons.Select( x => $"{x.First}{x.Second}{x.Third}")), path);

            return new DrawResponse() { PathToImage = path };
        }

        private T2 AddToCache<T, T2>(T key, T2 value)
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
            }
            var expiraton = TimeSpan.FromHours(float.Parse(_configuration["SlidingExpirationInHours"]));
            var opts = new MemoryCacheEntryOptions() { SlidingExpiration = expiraton };
            opts.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration() { EvictionCallback = OnEvicition});
            return _cache.Set(key, value, opts);
        }
    }
}
