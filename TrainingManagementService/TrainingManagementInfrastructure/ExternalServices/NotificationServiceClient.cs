using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementApplication.Interfaces;

namespace TrainingManagementInfrastructure.ExternalServices
{
    public class NotificationServiceClient : INotificationServiceClient
    {
        private readonly HttpClient _httpClient;

        public NotificationServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendNotificationAsync(
            Guid userId, string title, string content, int type, string bearerToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", bearerToken);

            var payload = new
            {
                userId = userId,
                title = title,
                content = content,
                type = type
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/notifications", payload);
                // Ne bacamo gresku ako ne uspe - notifikacija je "nice to have",
                // ne sme da sprecava glavnu operaciju (approve/reject)
                if (!response.IsSuccessStatusCode)
                {
                    // Opciono: loguj gresku
                    Console.WriteLine($"Notifikacija nije poslata. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // NotificationService nedostupan - glavna operacija ipak uspeva
                Console.WriteLine($"Greska pri slanju notifikacije: {ex.Message}");
            }
        }
    }
}
