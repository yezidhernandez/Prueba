using Mediator;
using Moq;
using PiedraAzul.Application.Common.Models.Patients;
using PiedraAzul.Application.Features.Appointments.CreateAppointment;
using PiedraAzul.Application.Features.Patients.Commands.CreateGuestPatient;
using PiedraAzul.Domain.Entities.Profiles.Doctor;
using PiedraAzul.Domain.Entities.Profiles.Patients;
using PiedraAzul.Domain.Repositories;

namespace PiedraAzul.Test.Tests;

public class AppointmentServiceTests
{
    [Fact]
    public async Task CreateAppointment_WithExistingGuest_CreatesAppointmentWithoutMediatorCall()
    {
        var appointmentRepo = new Mock<IAppointmentRepository>();
        var doctorRepo = new Mock<IDoctorRepository>();
        var slotRepo = new Mock<IDoctorAvailabilitySlotRepository>();
        var patientRepo = new Mock<IPatientRepository>();
        var patientGuestRepo = new Mock<IPatientGuestRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mediator = new Mock<IMediator>();

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var doctor = new Doctor("doctor-1", Domain.Entities.Shared.Enums.DoctorType.NaturalMedicine, "LIC-1", "");
        var slot = new DoctorAvailabilitySlot("doctor-1", date.DayOfWeek, new TimeSpan(9, 0, 0), new TimeSpan(9, 30, 0));
        var guest = new GuestPatient("guest-1", "Guest", "300", "info");

        doctorRepo.Setup(r => r.GetByIdAsync("doctor-1", It.IsAny<CancellationToken>())).ReturnsAsync(doctor);
        slotRepo.Setup(r => r.GetByIdAsync(slot.Id, It.IsAny<CancellationToken>())).ReturnsAsync(slot);
        patientGuestRepo.Setup(r => r.GetByIdAsync("123", It.IsAny<CancellationToken>())).ReturnsAsync(guest);
        appointmentRepo.Setup(r => r.ExistsBySlotAndDateAsync(slot.Id, date, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        unitOfWork.Setup(u => u.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<Domain.Entities.Operations.Appointment>>>(), It.IsAny<CancellationToken>()))
            .Returns((Func<CancellationToken, Task<Domain.Entities.Operations.Appointment>> action, CancellationToken ct) => action(ct));

        Domain.Entities.Operations.Appointment? added = null;
        appointmentRepo.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Operations.Appointment>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.Operations.Appointment, CancellationToken>((a, _) => added = a)
            .Returns(Task.CompletedTask);

        var sut = new CreateAppointmentHandler(
            appointmentRepo.Object,
            doctorRepo.Object,
            slotRepo.Object,
            patientRepo.Object,
            patientGuestRepo.Object,
            unitOfWork.Object,
            mediator.Object);

        var command = new CreateAppointmentCommand(
            "doctor-1",
            slot.Id,
            date,
            null,
            new GuestPatientRequest { Identification = "123", Name = "Guest", Phone = "300", ExtraInfo = "info" });

        var result = await sut.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("guest-1", result.PatientGuestId);
        Assert.Null(result.PatientUserId);
        Assert.NotNull(added);

        mediator.Verify(m => m.Send(It.IsAny<CreateGuestPatientCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
