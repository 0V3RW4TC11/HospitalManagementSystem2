using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Persistence.AppConstants;
using Persistence.Handlers.Identity;
using Queries.Identity;
using System.Security.Claims;

namespace Persistence.Tests.Query.Handlers
{
    [TestFixture]
    internal class IdentityQueryHandlerTests
    {
        [Test]
        public async Task HandleGetHmsUserIdFromCurrentUserQuery_NoUserSignedIn_ThrowsException()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            var signInManagerMock = CreateSignInManagerMock(null, userManagerMock.Object);
            signInManagerMock.Setup(x => x.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(false);
            var handler = new IdentityQueryHandler(signInManagerMock.Object, userManagerMock.Object);

            // Act
            var ex = Assert.ThrowsAsync<Exception>(async () => await handler.Handle(new GetHmsUserIdFromCurrentUserQuery(), CancellationToken.None));
            
            // Assert
            Assert.That(ex?.Message, Is.EqualTo("No user is currently signed in."));
        }

        [Test]
        public async Task HandleGetHmsUserIdFromCurrentUserQuery_UserSignedInButNoNameIdentifierClaim_ThrowsException()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            List<Claim> claims = [];
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var signInManagerMock = CreateSignInManagerMock(principal, userManagerMock.Object);
            signInManagerMock.Setup(x => x.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(true);
            var handler = new IdentityQueryHandler(signInManagerMock.Object, userManagerMock.Object);

            // Act
            var ex = Assert.ThrowsAsync<Exception>(async () => await handler.Handle(new GetHmsUserIdFromCurrentUserQuery(), CancellationToken.None));

            // Assert
            Assert.That(ex?.Message, Is.EqualTo("Signed in user has no Asp Identity claim"));
        }

        [Test]
        public async Task HandleGetHmsUserIdFromCurrentUserQuery_UserSignedInButIdentityUserNotFound_ThrowsException()
        {
            // Arrange
            var userManagerMock = CreateUserManagerMock();
            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser?)null);
            var identityUserId = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, identityUserId),
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var signInManagerMock = CreateSignInManagerMock(principal, userManagerMock.Object);
            signInManagerMock.Setup(x => x.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(true);
            var handler = new IdentityQueryHandler(signInManagerMock.Object, userManagerMock.Object);

            // Act
            var ex = Assert.ThrowsAsync<Exception>(async () => await handler.Handle(new GetHmsUserIdFromCurrentUserQuery(), CancellationToken.None));

            // Assert
            Assert.That(ex?.Message, Is.EqualTo("No Asp Identity found for ID " + identityUserId));
        }

        [Test]
        public async Task HandleGetHmsUserIdFromCurrentUserQuery_UserSignedInButNoHmsUserIdClaim_ThrowsException()
        {
            // Arrange
            var identityUser = new IdentityUser { Id = Guid.NewGuid().ToString() };
            List<Claim> userClaims = [];
            var userManagerMock = CreateUserManagerMock();
            userManagerMock.Setup(x => x.FindByIdAsync(It.Is(identityUser.Id, StringComparer.OrdinalIgnoreCase)))
                .ReturnsAsync(identityUser);
            userManagerMock.Setup(x => x.GetClaimsAsync(It.Is<IdentityUser>(x => x == identityUser)))
                .ReturnsAsync(userClaims);
            var contextClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, identityUser.Id),
            };
            var identity = new ClaimsIdentity(contextClaims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var signInManagerMock = CreateSignInManagerMock(principal, userManagerMock.Object);
            signInManagerMock.Setup(x => x.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(true);
            var handler = new IdentityQueryHandler(signInManagerMock.Object, userManagerMock.Object);

            // Act
            var ex = Assert.ThrowsAsync<Exception>(async () => await handler.Handle(new GetHmsUserIdFromCurrentUserQuery(), CancellationToken.None));

            // Assert
            Assert.That(ex?.Message, Is.EqualTo("User has no associated HMS User Id"));
        }

        [Test]
        public async Task HandleGetHmsUserIdFromCurrentUserQuery_UserSignedInWithHmsUserIdClaim_ReturnsGuid()
        {
            // Arrange
            var identityUser = new IdentityUser { Id = Guid.NewGuid().ToString() };
            var hmsUserId = Guid.NewGuid();
            var identUserClaims = new List<Claim>
            {
                new Claim(ClaimConstants.HmsUserId, hmsUserId.ToString())
            };
            var userManagerMock = CreateUserManagerMock();
            userManagerMock.Setup(x => x.FindByIdAsync(It.Is(identityUser.Id, StringComparer.OrdinalIgnoreCase)))
                .ReturnsAsync(identityUser);
            userManagerMock.Setup(x => x.GetClaimsAsync(It.Is<IdentityUser>(x => x == identityUser)))
                .ReturnsAsync(identUserClaims);
            var contextClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, identityUser.Id),
            };
            var identity = new ClaimsIdentity(contextClaims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var signInManagerMock = CreateSignInManagerMock(principal, userManagerMock.Object);
            signInManagerMock.Setup(x => x.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(true);
            var handler = new IdentityQueryHandler(signInManagerMock.Object, userManagerMock.Object);

            // Act
            var result = await handler.Handle(new GetHmsUserIdFromCurrentUserQuery(), CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(hmsUserId));
        }

        private static Mock<UserManager<IdentityUser>> CreateUserManagerMock()
        {
            return new Mock<UserManager<IdentityUser>>(new Mock<IUserStore<IdentityUser>>().Object, null, null, null, null, null, null, null, null);
        }

        private static Mock<SignInManager<IdentityUser>> CreateSignInManagerMock(ClaimsPrincipal? principal, UserManager<IdentityUser> userManager)
        {
            var httpContext = new DefaultHttpContext { User = principal };
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            contextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            return new Mock<SignInManager<IdentityUser>>(
                userManager,
                contextAccessorMock.Object,
                new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<IdentityUser>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object);
        }
    }
}