using Drawing.Models;
using DrawingService;
using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;

namespace Drawing.Services
{
    public class DrawingServiceImpl(IMemoryCache cache, IConfiguration configuration, ILogger<DrawingServiceImpl> logger) : Drawer.DrawerBase
    {
        private readonly IMemoryCache _cache = cache;

        private readonly IConfiguration _configuration = configuration;

        private readonly ILogger<DrawingServiceImpl> _logger = logger;

        public override async Task<DrawResponse> Draw(DrawRequest request, ServerCallContext context)
        {
            string requestKey = RequestToCacheKeyString(request);
            if (_cache.TryGetValue(requestKey, out string? cachedPath))
            {
                if (File.Exists(cachedPath))
                {
                    _logger.LogInformation($"Returned cached image with key '{requestKey}', path '{cachedPath}'");
                    return new DrawResponse() { PathToImage = cachedPath };
                }
                _cache.Remove(requestKey);
            }

            var set = RequestToInit(request);

            string path = $"/images/{Guid.NewGuid()}.png";

            await Task.Run(() => GeneralDrawer.Draw(set, PhysicalStyle.ToPhisycal(request.DrawStyle), path));

            AddToCache(requestKey, path);

            _logger.LogInformation($"Created image with key '{requestKey}', path {path}");

            return new DrawResponse() { PathToImage = path };
        }

        private void AddToCache(string key, string value)
        {
            void OnEvicition(object key, object? value, EvictionReason reason, object? state)
            {
                if (value is string path)
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch { }
                }
            }
            var expiraton = TimeSpan.FromHours(float.Parse(_configuration["SlidingExpirationInHours"]));
            var opts = new MemoryCacheEntryOptions() { SlidingExpiration = expiraton };
            opts.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration() { EvictionCallback = OnEvicition });
            _cache.Set(key, value, opts);
        }

        private static string RequestToCacheKeyString(DrawRequest request)
        {
            return request.DrawStyle.ToString() + string.Join("", request.Lessons.Select(x => $"{x.First}{x.Second}{x.Third}"));
        }

        private static SortedSet<DrawRow> RequestToInit(DrawRequest request)
        {
            var initalizer = request.Lessons.GroupBy(x => x.LessonNumber).Select(x =>
            {
                if (x.Count() >= 2)
                {
                    return new DrawRow()
                    {
                        IsDivided = true,
                        FirstLesson = x.FirstOrDefault(x => x.Subgroup == 1) ?? x.First(),
                        SecondLesson = x.FirstOrDefault(x => x.Subgroup == 2) ?? x.Last(),
                        Number = x.First().LessonNumber
                    };
                }
                else if (x.First().Subgroup == 1 || x.First().Subgroup == 2)
                {
                    var firstLesson = x.FirstOrDefault(x => x.Subgroup == 1);
                    var secondLesson = x.FirstOrDefault(x => x.Subgroup == 2);

                    return new DrawRow()
                    {
                        IsDivided = true,
                        FirstLesson = firstLesson,
                        SecondLesson = secondLesson,
                        Number = x.First().LessonNumber,
                    };
                }
                else
                {
                    return new DrawRow()
                    {
                        IsDivided = false,
                        FirstLesson = x.First(),
                        Number = x.First().LessonNumber,
                    };
                }
            });
            return new SortedSet<DrawRow>(initalizer.OrderBy(x => x.Number));
        }
    }
}
