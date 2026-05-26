using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceApplication.Interfaces
{
    public interface ITrainingServiceClient
    {
        Task<bool> HasClientTrainedWithTrainerAsync(Guid clientId, Guid trainerId, string bearerToken);
    }
}
