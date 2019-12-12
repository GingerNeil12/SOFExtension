using Newtonsoft.Json;
using SOFExtension.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SOFExtension.Services
{
    public class Client
    {
        private const string BASE_URI = "https://api.stackexchange.com/2.2/";
        private readonly HttpClientHandler _handler;

        private HttpClient _client;

        public Client()
        {
            _handler = new HttpClientHandler() {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
        }

        public async Task<SOFSearchModel> GetSearchResultsAsync(string search)
        {
            var stringResult = string.Empty;
            using(_client = new HttpClient(_handler)) {
                _client.BaseAddress = new Uri( BASE_URI );
                var uri = $"search?order=desc&sort=activity&site=stackoverflow&intitle={search}";
                var responseMessage = await _client.GetAsync( uri );
                using(var responseStream = await responseMessage.Content.ReadAsStreamAsync() ) {
                    using(var reader = new StreamReader(responseStream) ) {
                        stringResult = await reader.ReadToEndAsync();
                    }
                }
            }
            var result = JsonConvert.DeserializeObject<SOFSearchModel>( stringResult );
            return result;
        }

        public async Task<SOFQuestionModel> GetQuestionResultAsync(long id)
        {
            var stringResult = string.Empty;
            using(_client = new HttpClient( _handler ) ) {
                _client.BaseAddress = new Uri( BASE_URI );
                var uri = $"questions/{id}?order=desc&sort=activity&site=stackoverflow&filter=!9Z(-wwK0y";
                var responseMessage = await _client.GetAsync( uri );
                using(var responseStream = await responseMessage.Content.ReadAsStreamAsync() ) {
                    using(var reader = new StreamReader( responseStream ) ) {
                        stringResult = await reader.ReadToEndAsync();
                    }
                }
            }
            var result = JsonConvert.DeserializeObject<SOFQuestionModel>( stringResult );
            return result;
        }

        public async Task<SOFAnswerModel> GetAnswersResultAsync(long id)
        {
            var stringResult = string.Empty;
            using(_client = new HttpClient( _handler ) ) {
                _client.BaseAddress = new Uri( BASE_URI );
                var uri = $"questions/{id}/answers?order=desc&sort=activity&site=stackoverflow&filter=!9Z(-wzfpy";
                var responseMessage = await _client.GetAsync( uri );
                using(var responseStream = await responseMessage.Content.ReadAsStreamAsync() ) {
                    using(var reader = new StreamReader( responseStream ) ) {
                        stringResult = await reader.ReadToEndAsync();
                    }
                }
            }
            var result = JsonConvert.DeserializeObject<SOFAnswerModel>( stringResult );
            return result;
        }
    }
}
