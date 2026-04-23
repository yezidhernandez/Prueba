using PiedraAzul.Domain.Common.Exceptions;
using PiedraAzul.Domain.Entities.Operations;
using PiedraAzul.Domain.Entities.Profiles.Doctor;

namespace PiedraAzul.Test.Tests;

public class JwtTokenServiceTests
{
    [Fact]
    public void AppointmentCreate_WhenDateDoesNotMatchSlotDay_ThrowsDomainException()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var wrongDay = today.AddDays(1);
        var slot = new DoctorAvailabilitySlot("doctor-1", today.DayOfWeek, new TimeSpan(10, 0, 0), new TimeSpan(10, 30, 0));

        var action = () => Appointment.Create(slot, wrongDay, "doctor-1", "patient-1", null);

        Assert.Throws<DomainException>(action);
    }
}
