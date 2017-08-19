namespace ForeignExchangeMac1.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Models;
    using Newtonsoft.Json;

    public class ApiService
    {
        public async Task<Response> GetRates()
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new 
                    Uri("http://apiexchangerates.azurewebsites.net");
                var controller = "/api/Rates";
                var response = await client.GetAsync(controller);
                var result = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                var list = JsonConvert.DeserializeObject<List<Rate>>(result);
                return new Response
                {
                    IsSuccess = true,
                    Result = list,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
    }
}
