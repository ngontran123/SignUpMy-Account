using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Net.WebRequestMethods;
using System.Threading;
using System.Net.Http.Headers;

namespace GSMMASTER.Services
{
    public class TelecomService
    {
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(8); // Allow up to 20 parallel requests
        private const string MVTApi = @"http://ntp_backend_dev.wifosoft.com/api/my-register/";
        private const string MVNPTApi ="https://api-myvnpt.vnpt.vn/mapi/services/";
        private const string LoginApi = @"http://ntp_backend_dev.wifosoft.com/api/auth/";
        public static async Task<T> PostApiMVT<T>(string entry_point, FormUrlEncodedContent data,string token)
        {
                var res = default(T);
                try
                {
                    var httpClient = new HttpClientHandler();
                    using (var client = new HttpClient(httpClient))
                    {
                       client.DefaultRequestHeaders.Add("Authorization",token);
                        string url = $"{MVTApi}{entry_point}";
                        var request = await client.PostAsync(url, data);
                        var response = request.Content.ReadAsStringAsync().Result;
                        JObject ob = JObject.Parse(response);
                        res = JsonConvert.DeserializeObject<T>(ob.ToString());
                    }
                }
                catch (Exception er)
                {
                MessageBox.Show(er.Message);
                }
                return res;
        }
        public static T PostApiMVNPT<T>(string entry_point,FormUrlEncodedContent data)
        {
            var res = default(T);
            try
            {
                var httpClient = new HttpClientHandler();
                using(var client=new HttpClient(httpClient))
                {

                    string url = $"{MVNPTApi}{entry_point}";
                    var request = client.PostAsync(url, data).Result;
                    var response = request.Content.ReadAsStringAsync().Result;
                }
            }
            catch(Exception er)
            {
                Console.WriteLine(er.Message);
            }
            return res;
        }
        public static async Task<T> GetUserInfoApi<T>(string entry_point,FormUrlEncodedContent data)
        {
            var res = default(T);
            try
            {
                var httpClient = new HttpClientHandler();
                using(var client=new HttpClient(httpClient))
                {   
                    string url = $"{LoginApi}{entry_point}";
                    var request = await client.PostAsync(url,data);
                    var response= request.Content.ReadAsStringAsync().Result;
                    JObject ob = JObject.Parse(response);
                    res=JsonConvert.DeserializeObject<T>(ob.ToString());
                }
            }
            catch(Exception er)
            {
                Console.WriteLine(er.Message);
            }
            return res;
        }
    }
}
