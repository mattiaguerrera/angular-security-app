using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTCApi.Model;
using PTCApi.EntityClasses;
using PTCApi.ManagerClasses;

namespace PTCApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : AppControllerBase
    {
        public SecurityController(ILogger<ProductController> logger,
          PtcDbContext context, JwtSettings settings)
        {
            _logger = logger;
            _DbContext = context;
            _settings = settings;
        }
        private readonly PtcDbContext _DbContext;
        private readonly ILogger<ProductController> _logger;
        private JwtSettings _settings;

        [HttpPost("Login")]
        public IActionResult Login([FromBody] AppUser user)
        {
            IActionResult ret = null;
            AppUserAuth auth = new AppUserAuth();
            SecurityManager mgr = new SecurityManager(
               _DbContext, auth, _settings);

            auth = (AppUserAuth)mgr.ValidateUser(user.UserName, user.Password);
            if (auth.IsAuthenticated)
            {
                ret = StatusCode(StatusCodes.Status200OK, auth);
            }
            else
            {
                ret = StatusCode(StatusCodes.Status404NotFound,
                                 "Invalid User Name/Password.");
            }

            return ret;
        }
    }
}
