using Moq;
using PiedraAzul.Application.Common.Interfaces;
using PiedraAzul.Application.Features.Users.Commands.CreateProfileForRole;

namespace PiedraAzul.Test.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task CreateProfileForRoleHandler_CallsIdentityService()
    {
        var identity = new Mock<IIdentityService>();
        var sut = new CreateProfileForRoleHandler(identity.Object);

        await sut.Handle(new CreateProfileForRoleCommand("user-1", "patient"), CancellationToken.None);

        identity.Verify(i => i.CreateProfileForRoleAsync("user-1", "patient"), Times.Once);
    }
}
