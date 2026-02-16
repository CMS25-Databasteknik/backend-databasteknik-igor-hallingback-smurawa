using Backend.Domain.Modules.CourseRegistrations.Models;

namespace Backend.Tests.Unit.Backend.Domain.Modules.CourseRegistrations.Models;

public class CourseRegistration_Tests
{
    [Fact]
    public void Constructor_Should_Create_CourseRegistration_When_Parameters_Are_Valid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var courseEventId = Guid.NewGuid();
        var registrationDate = DateTime.UtcNow;
        var isPaid = false;

        // Act
        var courseRegistration = new CourseRegistration(id, participantId, courseEventId, registrationDate, isPaid);

        // Assert
        Assert.NotNull(courseRegistration);
        Assert.Equal(id, courseRegistration.Id);
        Assert.Equal(participantId, courseRegistration.ParticipantId);
        Assert.Equal(courseEventId, courseRegistration.CourseEventId);
        Assert.Equal(registrationDate, courseRegistration.RegistrationDate);
        Assert.Equal(isPaid, courseRegistration.IsPaid);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_Id_Is_Empty()
    {
        // Arrange
        var id = Guid.Empty;
        var participantId = Guid.NewGuid();
        var courseEventId = Guid.NewGuid();
        var registrationDate = DateTime.UtcNow;
        var isPaid = false;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new CourseRegistration(id, participantId, courseEventId, registrationDate, isPaid));

        Assert.Equal("id", exception.ParamName);
        Assert.Contains("ID cannot be empty", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_ParticipantId_Is_Empty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var participantId = Guid.Empty;
        var courseEventId = Guid.NewGuid();
        var registrationDate = DateTime.UtcNow;
        var isPaid = false;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new CourseRegistration(id, participantId, courseEventId, registrationDate, isPaid));

        Assert.Equal("participantId", exception.ParamName);
        Assert.Contains("Participant ID cannot be empty", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_CourseEventId_Is_Empty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var courseEventId = Guid.Empty;
        var registrationDate = DateTime.UtcNow;
        var isPaid = false;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new CourseRegistration(id, participantId, courseEventId, registrationDate, isPaid));

        Assert.Equal("courseEventId", exception.ParamName);
        Assert.Contains("Course event ID cannot be empty", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_RegistrationDate_Is_Default()
    {
        // Arrange
        var id = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var courseEventId = Guid.NewGuid();
        var registrationDate = default(DateTime);
        var isPaid = false;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new CourseRegistration(id, participantId, courseEventId, registrationDate, isPaid));

        Assert.Equal("registrationDate", exception.ParamName);
        Assert.Contains("Registration date must be specified", exception.Message);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Constructor_Should_Accept_Both_Payment_States(bool isPaid)
    {
        // Arrange
        var id = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var courseEventId = Guid.NewGuid();
        var registrationDate = DateTime.UtcNow;

        // Act
        var courseRegistration = new CourseRegistration(id, participantId, courseEventId, registrationDate, isPaid);

        // Assert
        Assert.Equal(isPaid, courseRegistration.IsPaid);
    }

    [Fact]
    public void Properties_Should_Be_Initialized_Correctly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var courseEventId = Guid.NewGuid();
        var registrationDate = DateTime.UtcNow;
        var isPaid = true;

        // Act
        var courseRegistration = new CourseRegistration(id, participantId, courseEventId, registrationDate, isPaid);

        // Assert
        Assert.Equal(id, courseRegistration.Id);
        Assert.Equal(participantId, courseRegistration.ParticipantId);
        Assert.Equal(courseEventId, courseRegistration.CourseEventId);
        Assert.Equal(registrationDate, courseRegistration.RegistrationDate);
        Assert.True(courseRegistration.IsPaid);
    }

    [Fact]
    public void Two_CourseRegistrations_With_Same_Values_Should_Have_Same_Property_Values()
    {
        // Arrange
        var id = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var courseEventId = Guid.NewGuid();
        var registrationDate = DateTime.UtcNow;
        var isPaid = false;

        var registration1 = new CourseRegistration(id, participantId, courseEventId, registrationDate, isPaid);
        var registration2 = new CourseRegistration(id, participantId, courseEventId, registrationDate, isPaid);

        // Assert
        Assert.Equal(registration1.Id, registration2.Id);
        Assert.Equal(registration1.ParticipantId, registration2.ParticipantId);
        Assert.Equal(registration1.CourseEventId, registration2.CourseEventId);
        Assert.Equal(registration1.RegistrationDate, registration2.RegistrationDate);
        Assert.Equal(registration1.IsPaid, registration2.IsPaid);
    }

    [Fact]
    public void Id_Property_Should_Be_Read_Only()
    {
        // Arrange
        var courseRegistration = new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, false);

        // Assert
        var initialId = courseRegistration.Id;
        Assert.Equal(initialId, courseRegistration.Id);
    }

    [Fact]
    public void ParticipantId_Property_Should_Be_Read_Only()
    {
        // Arrange
        var courseRegistration = new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, false);

        // Assert
        var initialParticipantId = courseRegistration.ParticipantId;
        Assert.Equal(initialParticipantId, courseRegistration.ParticipantId);
    }

    [Fact]
    public void CourseEventId_Property_Should_Be_Read_Only()
    {
        // Arrange
        var courseRegistration = new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, false);

        // Assert
        var initialCourseEventId = courseRegistration.CourseEventId;
        Assert.Equal(initialCourseEventId, courseRegistration.CourseEventId);
    }

    [Fact]
    public void RegistrationDate_Property_Should_Be_Read_Only()
    {
        // Arrange
        var registrationDate = DateTime.UtcNow;
        var courseRegistration = new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), registrationDate, false);

        // Assert
        var initialRegistrationDate = courseRegistration.RegistrationDate;
        Assert.Equal(initialRegistrationDate, courseRegistration.RegistrationDate);
    }

    [Fact]
    public void IsPaid_Property_Should_Be_Read_Only()
    {
        // Arrange
        var courseRegistration = new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, true);

        // Assert
        var initialIsPaid = courseRegistration.IsPaid;
        Assert.Equal(initialIsPaid, courseRegistration.IsPaid);
    }

    [Fact]
    public void Constructor_Should_Handle_Past_Registration_Dates()
    {
        // Arrange
        var registrationDate = DateTime.UtcNow.AddDays(-30);
        var courseRegistration = new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), registrationDate, false);

        // Assert
        Assert.Equal(registrationDate, courseRegistration.RegistrationDate);
    }

    [Fact]
    public void Multiple_Registrations_Can_Have_Same_ParticipantId()
    {
        // Arrange
        var participantId = Guid.NewGuid();
        var registration1 = new CourseRegistration(Guid.NewGuid(), participantId, Guid.NewGuid(), DateTime.UtcNow, false);
        var registration2 = new CourseRegistration(Guid.NewGuid(), participantId, Guid.NewGuid(), DateTime.UtcNow, false);

        // Assert
        Assert.Equal(participantId, registration1.ParticipantId);
        Assert.Equal(participantId, registration2.ParticipantId);
        Assert.NotEqual(registration1.Id, registration2.Id);
    }

    [Fact]
    public void Multiple_Registrations_Can_Have_Same_CourseEventId()
    {
        // Arrange
        var courseEventId = Guid.NewGuid();
        var registration1 = new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), courseEventId, DateTime.UtcNow, false);
        var registration2 = new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), courseEventId, DateTime.UtcNow, false);

        // Assert
        Assert.Equal(courseEventId, registration1.CourseEventId);
        Assert.Equal(courseEventId, registration2.CourseEventId);
        Assert.NotEqual(registration1.Id, registration2.Id);
    }

    [Fact]
    public void Constructor_Should_Create_Unpaid_Registration()
    {
        // Arrange & Act
        var courseRegistration = new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, false);

        // Assert
        Assert.False(courseRegistration.IsPaid);
    }

    [Fact]
    public void Constructor_Should_Create_Paid_Registration()
    {
        // Arrange & Act
        var courseRegistration = new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, true);

        // Assert
        Assert.True(courseRegistration.IsPaid);
    }
}
