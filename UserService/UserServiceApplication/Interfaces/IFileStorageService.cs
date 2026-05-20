using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceApplication.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveUserImage(Guid userId, IFormFile file);
    }
}
