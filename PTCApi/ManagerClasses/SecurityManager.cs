using System;
using System.Collections.Generic;
using System.Linq;
using PTCApi.Model;
using PTCApi.EntityClasses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace PTCApi.ManagerClasses
{
    public class SecurityManager
    {
        public SecurityManager(PtcDbContext context,
                               UserAuthBase auth, JwtSettings settings)
        {
            _DbContext = context;
            _auth = auth;
            _settings = settings;
        }

        private PtcDbContext _DbContext = null;
        private UserAuthBase _auth = null;
        private JwtSettings _settings = null;

        protected List<UserClaim> GetUserClaims(Guid userId)
        {
            List<UserClaim> list = new List<UserClaim>();

            try
            {
                list = _DbContext.Claims.Where(u => u.UserId == userId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Exception trying to retrieve user claims.", ex);
            }

            return list;
        }

        protected string BuildJwtToken(IList<UserClaim> claims, string userName)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.Key));

            // Create standard JWT claims
            List<Claim> jwtClaims = new List<Claim>
      {
        new Claim(JwtRegisteredClaimNames.Sub, userName),
        new Claim(JwtRegisteredClaimNames.Jti,
                  Guid.NewGuid().ToString())
      };

            // Add custom claims
            foreach (UserClaim claim in claims)
            {
                jwtClaims.Add(new Claim(claim.ClaimType,
                                        claim.ClaimValue));
            }

            // Create the JwtSecurityToken object
            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: jwtClaims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(
                       _settings.MinutesToExpiration),
                signingCredentials: new SigningCredentials(key,
                            SecurityAlgorithms.HmacSha256)
            );

            // Create a string representation of the Jwt token
            return new JwtSecurityTokenHandler().WriteToken(token); ;
        }

        protected UserAuthBase BuildUserAuthObject(Guid userId, string userName)
        {
            Type _authType = _auth.GetType();

            // Set User Properties
            _auth.UserId = userId;
            _auth.UserName = userName;
            _auth.IsAuthenticated = true;

            // Get all claims for this user
            _auth.Claims = GetUserClaims(userId);

            // Create JWT Bearer Token
            _auth.BearerToken = BuildJwtToken(_auth.Claims, userName);

            return _auth;
        }

        public UserAuthBase ValidateUser(string userName, string password)
        {
            List<UserBase> list = new List<UserBase>();

            try
            {
                list = _DbContext.Users.Where(
                  u => u.UserName.ToLower() == userName.ToLower()
                  && u.Password.ToLower() ==
                      password.ToLower()).ToList();

                if (list.Count() > 0)
                {
                    _auth = BuildUserAuthObject(list[0].UserId, userName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Exception while trying to retrieve user.", ex);
            }

            return _auth;
        }

    }
}
