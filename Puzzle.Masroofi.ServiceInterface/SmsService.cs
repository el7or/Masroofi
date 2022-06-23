using Microsoft.Extensions.Configuration;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.Core.ViewModels.Sms;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Puzzle.Masroofi.ServiceInterface
{
    public interface ISmsService
    {
        Task<SmsResponseViewModel> SendAsync(string mobileNumber, string message);
    }
    public class SmsService: ISmsService
    {
        private readonly UserIdentity _userIdentity;
        private readonly IConfiguration _configuration;
        private readonly string baseUrl;
        private readonly string username;
        private readonly string password;
        private readonly string sender;

        public SmsService(UserIdentity userIdentity, IConfiguration configuration)
        {
            _userIdentity = userIdentity;
            _configuration = configuration;
            baseUrl = _configuration.GetSection("SMSMisr:baseURL").Value;
            username = _configuration.GetSection("SMSMisr:username").Value;
            password = _configuration.GetSection("SMSMisr:password").Value;
            sender = _configuration.GetSection("SMSMisr:sender").Value;
        }

        public async Task<SmsResponseViewModel> SendAsync(string mobileNumber, string message)
        {
            var smsRequest = new SmsRequestViewModel
            {
                Username = username,
                Password = password,
                Sender = sender,
                Language = (int)_userIdentity.Language.Value,
                Mobile = mobileNumber,
                Message = message
            };
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(baseUrl,
                    new StringContent(JsonSerializer.Serialize(smsRequest), Encoding.UTF8, "application/json"));
                var result =  await response.Content.ReadFromJsonAsync<SmsResponseViewModel>();
                return result;
            }
        }
    }
}
