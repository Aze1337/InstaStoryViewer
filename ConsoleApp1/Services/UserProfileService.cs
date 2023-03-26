using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class UserProfileService : BaseService
    {
        public UserProfileService(HttpClient httpClient) : base(httpClient) { }



        public async Task<(string userId, string csrf)> GetUserIdAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Failed to load the page");
            }

            var content = await response.Content.ReadAsStringAsync();
            var first_pos = content.IndexOf("profile_id");
            first_pos = content.IndexOf(":", first_pos);
            first_pos = content.IndexOf('"', first_pos) + 1;
            var second_pos = content.IndexOf('"', first_pos);
            var profilePageId = content.Substring(first_pos, (second_pos - first_pos));

            string pattern = @"\\\""csrf_token\\\"":\\\""([^\\]+)\\\""";
            Match match = Regex.Match(content, pattern);
            string csrfToken = "";
            if (match.Success)
            {
                csrfToken = match.Groups[1].Value;
            }
            else
            {
                Console.WriteLine("No match found");
            }
            first_pos = content.IndexOf("config\":{\"csrf_token");
            first_pos = content.IndexOf("csrf_token", first_pos);
            first_pos = content.IndexOf(":", first_pos);
            first_pos = content.IndexOf('"', first_pos) + 1;
            second_pos = content.IndexOf('"', first_pos);
            return (profilePageId, csrfToken);
        }
    }
}
