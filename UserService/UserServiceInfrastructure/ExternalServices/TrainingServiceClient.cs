using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UserServiceApplication.Interfaces;

namespace UserServiceInfrastructure.ExternalServices
{
    public class TrainingServiceClient : ITrainingServiceClient
    {
        private readonly HttpClient _httpClient;

        public TrainingServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> HasClientTrainedWithTrainerAsync(
            Guid clientId, Guid trainerId, string bearerToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", bearerToken);

            var url = $"/api/internal/has-trained?clientId={clientId}&trainerId={trainerId}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return false;

                var result = await response.Content.ReadFromJsonAsync<HasTrainedResponse>();
                return result?.HasTrained ?? false;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

     

        // Pomocna klasa za deserializaciju JSON odgovora
        private class HasTrainedResponse
        {
            public bool HasTrained { get; set; }
        }
    }
}
