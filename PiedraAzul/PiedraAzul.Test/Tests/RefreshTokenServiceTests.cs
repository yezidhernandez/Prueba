using PiedraAzul.Domain.Common.Exceptions;
using PiedraAzul.Domain.Entities.Profiles.Doctor;

namespace PiedraAzul.Test.Tests;

public class RefreshTokenServiceTests
{
    [Fact]
    public void DoctorAvailabilitySlot_WithInvalidRange_ThrowsDomainException()
    {
        var action = () => new DoctorAvailabilitySlot(
            "doctor-1",
            DayOfWeek.Monday,
            new TimeSpan(11, 0, 0),
            new TimeSpan(10, 0, 0));

        Assert.Throws<DomainException>(action);
    }
}
