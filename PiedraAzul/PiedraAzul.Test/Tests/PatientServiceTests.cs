using Moq;
using PiedraAzul.Application.Features.Patients.Commands.CreateGuestPatient;
using PiedraAzul.Domain.Entities.Profiles.Patients;
using PiedraAzul.Domain.Repositories;

namespace PiedraAzul.Test.Tests;

public class PatientServiceTests
{
    [Fact]
    public async Task CreateGuestPatientHandler_AddsGuestAndReturnsGeneratedId()
    {
        var repo = new Mock<IPatientGuestRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        unitOfWork
            .Setup(u => u.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<string>>>(), It.IsAny<CancellationToken>()))
            .Returns((Func<CancellationToken, Task<string>> action, CancellationToken ct) => action(ct));

        GuestPatient? capturedGuest = null;

        repo
            .Setup(r => r.AddAsync(It.IsAny<GuestPatient>(), It.IsAny<CancellationToken>()))
            .Callback<GuestPatient, CancellationToken>((guest, _) => capturedGuest = guest)
            .Returns(Task.CompletedTask);

        var sut = new CreateGuestPatientHandler(repo.Object, unitOfWork.Object);

        var result = await sut.Handle(
            new CreateGuestPatientCommand("100200300", "Paciente Invitado", "3001234567", "Observaciones"),
            CancellationToken.None);

        Assert.False(string.IsNullOrWhiteSpace(result));
        Assert.NotNull(capturedGuest);
        Assert.Equal("Paciente Invitado", capturedGuest!.Name);
        Assert.Equal("3001234567", capturedGuest.Phone);
        Assert.Equal("Observaciones", capturedGuest.ExtraInfo);

        repo.Verify(r => r.AddAsync(It.IsAny<GuestPatient>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
