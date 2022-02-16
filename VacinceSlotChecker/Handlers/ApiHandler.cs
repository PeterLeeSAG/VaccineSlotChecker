using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using VaccinceSlotChecker.Models;
using Newtonsoft.Json;

namespace VaccinceSlotChecker.Handlers
{
    public static class ApiHandler
    {

        //Execute the schedule job on the remote api
        public static async Task<CenterSlotResult> Execute(string centerID, string cvCenterType, string cvName)
        {
            CenterSlotResult result;
            var timeout = 10;
           
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(timeout);

                var requestURL = string.Format("https://bookingform.covidvaccine.gov.hk/forms/api_center?center_id={0}&cv_ctc_type={1}&cv_name={2}",centerID, cvCenterType, cvName);

                //Console.WriteLine("URL: " + requestURL);
                //Console.WriteLine("URL: " + requestURL);
                HttpContent httpContent = new StringContent("", Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync(requestURL, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();

                    HttpContent content = response.Content;
                    var headers = content.Headers;                    
                    result = JsonConvert.DeserializeObject<CenterSlotResult>(responseString);
                }
                else
                {
                    Console.WriteLine("Call API failed! Server's code:" + response.StatusCode.ToString());
                    result = null;
                }

                //HttpResponseMessage response = await client.GetAsync(requestURL);
                return result;
            }
        }
    }
}