using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Commex_Unit_Tests.IntegrationTests.Fixtures
{
    public class TestAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public string Role { get; set; } = "User";
        public string UserId { get; set; } = "test-user-id";
        public string UserName { get; set; } = "Test User";
    }

    public class TestAuthHandler : AuthenticationHandler<TestAuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<TestAuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Check if test auth should fail
            if (Request.Headers.ContainsKey("X-Test-Unauthorized"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
            }

            // Create test claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Options.UserId),
                new Claim(ClaimTypes.Name, Options.UserName),
                new Claim(ClaimTypes.Role, Options.Role),
                new Claim("sub", Options.UserId),
                new Claim("email", "test@example.com")
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}