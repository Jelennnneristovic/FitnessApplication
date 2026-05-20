using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceDomain.Enums;

namespace UserServiceApplication.DTOs.Requests
{
    public class UserFilterRequest
    {
        // nullable — ako se ne pošalju, ne filtriramo po njima.
        public string? Keyword { get; set; }
        public UserStatus? Status { get; set; }
    }
}
