using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
            var httpClient=HttpClientFactory.CreateClient("OurWebAPI");
           var result= await httpClient.PostAsJsonAsync("auth", new Credential {UserName="admin",Password="password" });
            result.EnsureSuccessStatusCode();
            string strJwt = await result.Content.ReadAsStringAsync();
            var token=JsonConvert.DeserializeObject<JwtToken>(strJwt);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
            WeatherForeCastItems= await httpClient.GetFromJsonAsync<List<WeatherForeCastDTO>>("WeatherForecast");
        }
    }
}
