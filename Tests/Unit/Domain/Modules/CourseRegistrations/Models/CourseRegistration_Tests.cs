using Backend.Domain.Modules.CourseRegistrations.Models;

namespace Backend.Tests.Unit.Domain.Modules.CourseRegistrations.Models;

public class CourseRegistration_Tests
{
    [Fact]
    public void Constructor_Should_Create_CourseRegistration_When_Parameters_Are_Valid()
    {
        var id = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var courseEventId = Guid.NewGuid();
        var registrationDate = DateTime.UtcNow;

        var courseRegistration = new CourseRegistration(
            id,
            participantId,
            courseEventId,
            registrationDate,
            CourseRegistrationStatus.Pending,
            PaymentMethod.Card);

        Assert.NotNull(courseRegistration);
        Assert.Equal(id, courseRegistration.Id);
        Assert.Equal(participantId, courseRegistration.ParticipantId);
        Assert.Equal(courseEventId, courseRegistration.CourseEventId);
        Assert.Equal(registrationDate, courseRegistration.RegistrationDate);
        Assert.Equal(CourseRegistrationStatus.Pending, courseRegistration.Status);
    }

    [Fact]
    public void Constructor_Should_Throw_When_Id_Is_Empty()
    {
        var participantId = Guid.NewGuid();
        var courseEventId = Guid.NewGuid();

        var ex = Assert.Throws<ArgumentException>(() =>
            new CourseRegistration(Guid.Empty, participantId, courseEventId, DateTime.UtcNow, CourseRegistrationStatus.Paid, PaymentMethod.Invoice));

        Assert.Equal("id", ex.ParamName);
        Assert.Contains("ID cannot be empty", ex.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_When_Status_Is_Invalid()
    {
        var invalidStatus = (CourseRegistrationStatus)42;

        var ex = Assert.Throws<ArgumentException>(() =>
            new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, invalidStatus, PaymentMethod.Cash));

        Assert.Equal("status", ex.ParamName);
        Assert.Contains("Registration status is invalid", ex.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_When_PaymentMethod_Is_Invalid()
    {
        var invalidPayment = (PaymentMethod)(-1);

        var ex = Assert.Throws<ArgumentException>(() =>
            new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, CourseRegistrationStatus.Paid, invalidPayment));

        Assert.Equal("paymentMethod", ex.ParamName);
        Assert.Contains("Payment method is invalid", ex.Message);
    }

    [Theory]
    [InlineData(CourseRegistrationStatus.Pending, PaymentMethod.Card)]
    [InlineData(CourseRegistrationStatus.Paid, PaymentMethod.Invoice)]
    [InlineData(CourseRegistrationStatus.Cancelled, PaymentMethod.Cash)]
    [InlineData(CourseRegistrationStatus.Refunded, PaymentMethod.Card)]
    public void Constructor_Should_Accept_All_Statuses(CourseRegistrationStatus status, PaymentMethod payment)
    {
        var registration = new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, status, payment);

        Assert.Equal(status, registration.Status);
        Assert.Equal(payment, registration.PaymentMethod);
    }

    [Fact]
    public void Properties_Should_Be_ReadOnly()
    {
        var registration = new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, CourseRegistrationStatus.Paid, PaymentMethod.Card);

        Assert.Equal(registration.Id, registration.Id);
        Assert.Equal(registration.ParticipantId, registration.ParticipantId);
        Assert.Equal(registration.CourseEventId, registration.CourseEventId);
        Assert.Equal(registration.RegistrationDate, registration.RegistrationDate);
        Assert.Equal(registration.Status, registration.Status);
    }

    [Fact]
    public void Constructor_Allows_Past_Dates()
    {
        var date = DateTime.UtcNow.AddDays(-10);

        var registration = new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), date, CourseRegistrationStatus.Paid, PaymentMethod.Cash);

        Assert.Equal(date, registration.RegistrationDate);
    }
}
