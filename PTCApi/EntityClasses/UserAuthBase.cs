using System;
using System.Collections.Generic;

namespace PTCApi.EntityClasses
{
    public class UserAuthBase
    {
        public UserAuthBase()
        {
            UserId = Guid.Empty;
            UserName = string.Empty;
            BearerToken = string.Empty;
            IsAuthenticated = false;
            Claims = new List<UserClaim>();
        }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string BearerToken { get; set; }
        public bool IsAuthenticated { get; set; }
        public List<UserClaim> Claims { get; set; }
    }
}
