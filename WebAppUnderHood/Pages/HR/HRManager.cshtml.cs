using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppUnderHood.DTO;

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
            WeatherForeCastItems= await httpClient.GetFromJsonAsync<List<WeatherForeCastDTO>>("WeatherForecast");
        }
    }
}
