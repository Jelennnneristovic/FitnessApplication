using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceApplication.DTOs;
using UserServiceApplication.DTOs.Requests;
using UserServiceApplication.DTOs.Responses;

namespace UserServiceApplication.Interfaces
{
    public interface IAuthService 
    {
       
        Task<Auth> RegisterAsync(RegisterUser request);
        Task<Auth> LoginAsync(LoginUser request);

    }
}
