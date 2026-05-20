using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceApplication.DTOs.Responses;
using DomainUser = UserServiceDomain.Entities.User;



namespace UserServiceApplication.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(DomainUser user);
    }
}
