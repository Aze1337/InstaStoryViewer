using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Services
{
    public class ReelsMediaService : BaseService
    {
        public ReelsMediaService(HttpClient httpClient) : base(httpClient)
        {
        }


        public async Task<(string mediaId, string latest_reel_media)> GetReelsMediaId(string reel_ids, string csrfToken, string sessionId)
        {
            string ds_user_id = "57470793652";
            string endpoint = $"https://www.instagram.com/api/v1/feed/reels_media/?reel_ids={reel_ids}";
            _httpClient.DefaultRequestHeaders.Add("X-CSRFToken", csrfToken);
            _httpClient.DefaultRequestHeaders.Add("X-IG-App-ID", "936619743392459");
            _httpClient.DefaultRequestHeaders.Add("Cookie", $"csrftoken={csrfToken}; sessionid={sessionId};");

            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                // Extract the desired string from the response content
                var first_pos = content.IndexOf("media_ids");
                first_pos = content.IndexOf("[", first_pos) + 2;
                var last_pos = content.IndexOf('"', first_pos);
                string anotherString = content.Substring(first_pos, last_pos - first_pos);
                first_pos = content.IndexOf("latest_reel_media");
                first_pos = content.IndexOf(':', first_pos) + 1;
                last_pos = content.IndexOf(',', first_pos);
                string latestReelMedia = content.Substring(first_pos, last_pos - first_pos);
                return (anotherString,latestReelMedia);
            }
            else
            {
                throw new HttpRequestException($"Error fetching reels media: {response.StatusCode}");
            }
        }

        public async Task MarkStoryAsSeenAsync(string mediaId,string userId,string reelTaken,string csrfToken,string sessionId)
        {
            string endpoint = "https://www.instagram.com/api/v1/stories/reel/seen";

            // Set necessary headers
            //_httpClient.DefaultRequestHeaders.Add("X-CSRFToken", csrfToken);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36");
            //_httpClient.DefaultRequestHeaders.Add("Cookie", $"csrftoken={csrfToken};");

            var postContent = new FormUrlEncodedContent(new[]
           {
                new KeyValuePair<string, string>("reelMediaId", mediaId),
                new KeyValuePair<string, string>("reelMediaOwnerId", userId),
                new KeyValuePair<string, string>("reelId", userId),
                new KeyValuePair<string, string>("reelMediaTakenAt", reelTaken),
                new KeyValuePair<string, string>("viewSeenAt", reelTaken)
            });

            // Make the POST request
            HttpResponseMessage response = await _httpClient.PostAsync(endpoint, postContent);
            string content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
            // Check response status
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error marking story as seen: {response.StatusCode}");
            }
        }
    }
}
