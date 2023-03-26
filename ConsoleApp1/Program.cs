using System;
using System.Threading.Tasks;
using ConsoleApp1.Services;
using Microsoft.Extensions.DependencyInjection;
namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = ConfigureServices();

            var userProfileService = serviceProvider.GetRequiredService<UserProfileService>();
            var reelsMediaService = serviceProvider.GetRequiredService<ReelsMediaService>();
            try
            {
                string sessionId = "";
                string userProfileUrl = "https://www.instagram.com/jena39425/";
                (string userId, string csrfToken) = await userProfileService.GetUserIdAsync(userProfileUrl);
                Console.WriteLine("user id - " + userId);
                Console.WriteLine("csrf-token - " + csrfToken);
                (string reelsMediaId, string latest_media) = await reelsMediaService.GetReelsMediaId(userId, csrfToken, sessionId);
                Console.WriteLine("reels media id - " + reelsMediaId);
                Console.WriteLine("latest media id - " + latest_media);
                await reelsMediaService.MarkStoryAsSeenAsync(reelsMediaId, userId, latest_media ,csrfToken,sessionId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: no viewable story");
            }
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddHttpClient<UserProfileService>();
            services.AddHttpClient<ReelsMediaService>();
            //services.AddHttpClient<StoryViewService>();

            return services.BuildServiceProvider();
        }
    }
}
