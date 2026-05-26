using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingManagementApplication.Interfaces
{
    public interface INotificationServiceClient
    {
        Task SendNotificationAsync(
            Guid userId,
            string title,
            string content,
            int type,
            string bearerToken);
    }
}
