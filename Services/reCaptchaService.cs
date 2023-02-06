using Newtonsoft.Json;
namespace FreshFarmMarket.Services
{
    public class reCaptchaService
    {
        public virtual async Task<reCaptchaResponse> tokenVerify(string token)
        {
            reCaptchaData data = new reCaptchaData
            {
                response = token,
                secret = "6LcW-lEkAAAAAD_UcmU15tlzpMtGshzZVf3OwK6G"
            };

            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={data.secret}&response={data.response}");
            var reCaptcharesponse = JsonConvert.DeserializeObject<reCaptchaResponse>(response);
            return reCaptcharesponse;
        }
    }
    public class reCaptchaData
    {
        public string response { get; set; }
        public string secret { get; set; }
    }
    public class reCaptchaResponse
    {
        public bool success { get; set; }
        public DateTime challenge_ts { get; set; }
        public string hostname { get; set; }
        public long score { get; set; }
    }
}
