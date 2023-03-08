using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Opmuntring
{
    public class OpenAiService
    {

        private static HttpWebRequest CreateRequest(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";

            return request;
        }

        private static string GetResponseAsString(WebResponse response)
        {
            var responseStream = response.GetResponseStream();
            var reader = new StreamReader(responseStream);
            string html = reader.ReadToEnd();
            return html;
        }


        public static async Task<string> promptGpt3Async(string prompt, int limit, string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            var request = new HttpRequestMessage();

            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri("https://api.openai.com/v1/completions");

            var json = JsonConvert.SerializeObject(new
            {
                model = "text-davinci-003",
                max_tokens = limit,
                temperature = 0.7,
                top_p = 1,
                prompt = prompt,
                frequency_penalty = 0,
                presence_penalty = 00
            });


            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject(responseText);
            string reason = obj.choices[0].finish_reason;
            if (reason == "length")
            {
                return await promptGpt3Async(prompt, limit * 2, token);
            }
            string text = obj.choices[0].text;
            return text;
        }
    }
}
