using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WebAppUnderHood.Authorization;
using WebAppUnderHood.DTO;
using WebAppUnderHood.Models;

namespace WebAppUnderHood.Pages.HR
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        [BindProperty]
        public List<WeatherForeCastDTO> WeatherForeCastItems { get; set; }
        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
        }

        public IHttpClientFactory HttpClientFactory { get; }

        public async Task OnGet()
        {
            WeatherForeCastItems = await InvokeEndPoint<List<WeatherForeCastDTO>>("OurWebAPI", "WeatherForecast");
        }
        private async Task<T> InvokeEndPoint<T>(string clientWebApiName,string url)
        {
            JwtToken token = null;

            //get token from session
            var strTokenObj = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrWhiteSpace(strTokenObj))
            {
                token = await Authenticate();
            }
            else
                token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj);

            if (token == null || string.IsNullOrWhiteSpace(token.AccessToken) || token.ExpiresAt <= DateTime.UtcNow)
                token = await Authenticate();

            var httpClient = HttpClientFactory.CreateClient(clientWebApiName);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
           return await httpClient.GetFromJsonAsync<T>(url);
        }
        private async Task<JwtToken> Authenticate()
        {
            var httpClient = HttpClientFactory.CreateClient("OurWebAPI");
            var result = await httpClient.PostAsJsonAsync("auth", new Credential { UserName = "admin", Password = "password" });
            result.EnsureSuccessStatusCode();
            string strJwt = await result.Content.ReadAsStringAsync();
            HttpContext.Session.SetString("access_token", strJwt);
           return JsonConvert.DeserializeObject<JwtToken>(strJwt);
        }
    }
}
