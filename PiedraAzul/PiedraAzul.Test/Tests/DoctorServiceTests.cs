using Moq;
using PiedraAzul.Application.Common.Interfaces;
using PiedraAzul.Application.Common.Models.User;
using PiedraAzul.Application.Features.Doctors.Queries.GetDoctorsBySpecialty;
using PiedraAzul.Domain.Entities.Profiles.Doctor;
using PiedraAzul.Domain.Entities.Shared.Enums;
using PiedraAzul.Domain.Repositories;

namespace PiedraAzul.Test.Tests;

public class DoctorServiceTests
{
    [Fact]
    public async Task GetDoctorsBySpecialty_ReturnsOnlyDoctorsWithMatchingIdentity()
    {
        var doctorRepo = new Mock<IDoctorRepository>();
        var identityService = new Mock<IIdentityService>();

        var d1 = new Doctor("doc-1", DoctorType.NaturalMedicine, "LIC-01", "Notas 1");
        var d2 = new Doctor("doc-2", DoctorType.NaturalMedicine, "LIC-02", "Notas 2");

        doctorRepo
            .Setup(r => r.GetBySpecialtyAsync(DoctorType.NaturalMedicine, It.IsAny<CancellationToken>()))
            .ReturnsAsync([d1, d2]);

        identityService
            .Setup(i => i.GetByIds(It.IsAny<List<string>>()))
            .ReturnsAsync([
                new UserDto { Id = "doc-1", Name = "Dra. Uno", AvatarUrl = "avatar-1" }
            ]);

        var sut = new GetDoctorsBySpecialtyHandler(doctorRepo.Object, identityService.Object);

        var result = await sut.Handle(new GetDoctorsBySpecialtyQuery(DoctorType.NaturalMedicine), CancellationToken.None);

        var doctorDto = Assert.Single(result);
        Assert.Equal("doc-1", doctorDto.Id);
        Assert.Equal("Dra. Uno", doctorDto.Name);
        Assert.Equal("LIC-01", doctorDto.LicenseNumber);
    }
}
