using Backend.Domain.Modules.CourseEvents.Models;
using Backend.Domain.Modules.VenueTypes.Models;

namespace Backend.Tests.Unit.Domain.Modules.CourseEvents.Models;

public class CourseEvent_Tests
{
    [Fact]
    public void Constructor_Should_Create_CourseEvent_When_Parameters_Are_Valid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var eventDate = DateTime.UtcNow;
        var price = 1000m;
        var seats = 30;
        var courseEventTypeId = 1;

        // Act
        var courseEvent = new CourseEvent(id, courseId, eventDate, price, seats, courseEventTypeId, VenueType.InPerson);

        // Assert
        Assert.NotNull(courseEvent);
        Assert.Equal(id, courseEvent.Id);
        Assert.Equal(courseId, courseEvent.CourseId);
        Assert.Equal(eventDate, courseEvent.EventDate);
        Assert.Equal(price, courseEvent.Price);
        Assert.Equal(seats, courseEvent.Seats);
        Assert.Equal(courseEventTypeId, courseEvent.CourseEventTypeId);
        Assert.Equal(VenueType.InPerson, courseEvent.VenueType);
    }

    [Theory]
    [InlineData(VenueType.InPerson)]
    [InlineData(VenueType.Online)]
    [InlineData(VenueType.Hybrid)]
    public void Constructor_Should_Accept_All_Valid_VenueTypes(VenueType venueType)
    {
        var courseEvent = new CourseEvent(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, 1000m, 30, 1, venueType);

        Assert.Equal(venueType, courseEvent.VenueType);
    }

    [Fact]
    public void Constructor_Should_Throw_When_VenueType_Invalid()
    {
        var invalidVenue = (VenueType)999;

        var ex = Assert.Throws<ArgumentException>(() =>
            new CourseEvent(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, 1000m, 30, 1, invalidVenue));

        Assert.Equal("venueType", ex.ParamName);
        Assert.Contains("invalid", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_Id_Is_Empty()
    {
        // Arrange
        var id = Guid.Empty;
        var courseId = Guid.NewGuid();
        var eventDate = DateTime.UtcNow;
        var price = 1000m;
        var seats = 30;
        var courseEventTypeId = 1;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new CourseEvent(id, courseId, eventDate, price, seats, courseEventTypeId, VenueType.InPerson));

        Assert.Equal("id", exception.ParamName);
        Assert.Contains("cannot be empty", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_CourseId_Is_Empty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var courseId = Guid.Empty;
        var eventDate = DateTime.UtcNow;
        var price = 1000m;
        var seats = 30;
        var courseEventTypeId = 1;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new CourseEvent(id, courseId, eventDate, price, seats, courseEventTypeId, VenueType.InPerson));

        Assert.Equal("courseId", exception.ParamName);
        Assert.Contains("cannot be empty", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_EventDate_Is_Default()
    {
        // Arrange
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var eventDate = default(DateTime);
        var price = 1000m;
        var seats = 30;
        var courseEventTypeId = 1;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new CourseEvent(id, courseId, eventDate, price, seats, courseEventTypeId, VenueType.InPerson));

        Assert.Equal("eventDate", exception.ParamName);
        Assert.Contains("Event date must be specified", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentOutOfRangeException_When_Price_Is_Negative()
    {
        // Arrange
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var eventDate = DateTime.UtcNow;
        var price = -1m;
        var seats = 30;
        var courseEventTypeId = 1;

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CourseEvent(id, courseId, eventDate, price, seats, courseEventTypeId, VenueType.InPerson));

        Assert.Equal("price", exception.ParamName);
        Assert.Contains("Price cannot be negative", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Accept_Zero_Price()
    {
        // Arrange
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var eventDate = DateTime.UtcNow;
        var price = 0m;
        var seats = 30;
        var courseEventTypeId = 1;

        // Act
        var courseEvent = new CourseEvent(id, courseId, eventDate, price, seats, courseEventTypeId, VenueType.InPerson);

        // Assert
        Assert.Equal(0m, courseEvent.Price);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentOutOfRangeException_When_Seats_Is_Zero()
    {
        // Arrange
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var eventDate = DateTime.UtcNow;
        var price = 1000m;
        var seats = 0;
        var courseEventTypeId = 1;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CourseEvent(id, courseId, eventDate, price, seats, courseEventTypeId, VenueType.InPerson));
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentOutOfRangeException_When_Seats_Is_Negative()
    {
        // Arrange
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var eventDate = DateTime.UtcNow;
        var price = 1000m;
        var seats = -1;
        var courseEventTypeId = 1;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CourseEvent(id, courseId, eventDate, price, seats, courseEventTypeId, VenueType.InPerson));
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentOutOfRangeException_When_CourseEventTypeId_Is_Zero()
    {
        // Arrange
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var eventDate = DateTime.UtcNow;
        var price = 1000m;
        var seats = 30;
        var courseEventTypeId = 0;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CourseEvent(id, courseId, eventDate, price, seats, courseEventTypeId, VenueType.InPerson));
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentOutOfRangeException_When_CourseEventTypeId_Is_Negative()
    {
        // Arrange
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var eventDate = DateTime.UtcNow;
        var price = 1000m;
        var seats = 30;
        var courseEventTypeId = -1;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CourseEvent(id, courseId, eventDate, price, seats, courseEventTypeId, VenueType.InPerson));
    }

    [Theory]
    [InlineData(500.00)]
    [InlineData(1000.00)]
    [InlineData(1500.50)]
    [InlineData(2000.99)]
    public void Constructor_Should_Create_CourseEvent_With_Various_Prices(decimal price)
    {
        // Arrange
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var eventDate = DateTime.UtcNow;
        var seats = 30;
        var courseEventTypeId = 1;

        // Act
        var courseEvent = new CourseEvent(id, courseId, eventDate, price, seats, courseEventTypeId, VenueType.InPerson);

        // Assert
        Assert.Equal(price, courseEvent.Price);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(25)]
    [InlineData(50)]
    [InlineData(100)]
    public void Constructor_Should_Create_CourseEvent_With_Various_Seat_Capacities(int seats)
    {
        // Arrange
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var eventDate = DateTime.UtcNow;
        var price = 1000m;
        var courseEventTypeId = 1;

        // Act
        var courseEvent = new CourseEvent(id, courseId, eventDate, price, seats, courseEventTypeId, VenueType.InPerson);

        // Assert
        Assert.Equal(seats, courseEvent.Seats);
    }

    [Fact]
    public void Properties_Should_Be_Initialized_Correctly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var eventDate = DateTime.UtcNow;
        var price = 1000m;
        var seats = 30;
        var courseEventTypeId = 1;

        // Act
        var courseEvent = new CourseEvent(id, courseId, eventDate, price, seats, courseEventTypeId, VenueType.InPerson);

        // Assert
        Assert.Equal(id, courseEvent.Id);
        Assert.Equal(courseId, courseEvent.CourseId);
        Assert.Equal(eventDate, courseEvent.EventDate);
        Assert.Equal(price, courseEvent.Price);
        Assert.Equal(seats, courseEvent.Seats);
        Assert.Equal(courseEventTypeId, courseEvent.CourseEventTypeId);
    }

    [Fact]
    public void Two_CourseEvents_With_Same_Values_Should_Have_Same_Property_Values()
    {
        // Arrange
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var eventDate = DateTime.UtcNow;
        var price = 1000m;
        var seats = 30;
        var courseEventTypeId = 1;

        var courseEvent1 = new CourseEvent(id, courseId, eventDate, price, seats, courseEventTypeId, VenueType.InPerson);
        var courseEvent2 = new CourseEvent(id, courseId, eventDate, price, seats, courseEventTypeId, VenueType.InPerson);

        // Assert
        Assert.Equal(courseEvent1.Id, courseEvent2.Id);
        Assert.Equal(courseEvent1.CourseId, courseEvent2.CourseId);
        Assert.Equal(courseEvent1.EventDate, courseEvent2.EventDate);
        Assert.Equal(courseEvent1.Price, courseEvent2.Price);
        Assert.Equal(courseEvent1.Seats, courseEvent2.Seats);
        Assert.Equal(courseEvent1.CourseEventTypeId, courseEvent2.CourseEventTypeId);
    }

    [Fact]
    public void Id_Property_Should_Be_Read_Only()
    {
        // Arrange
        var courseEvent = new CourseEvent(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, 1000m, 30, 1, VenueType.InPerson);

        // Assert
        var initialId = courseEvent.Id;
        Assert.Equal(initialId, courseEvent.Id);
    }

    [Fact]
    public void CourseId_Property_Should_Be_Read_Only()
    {
        // Arrange
        var courseEvent = new CourseEvent(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, 1000m, 30, 1, VenueType.InPerson);

        // Assert
        var initialCourseId = courseEvent.CourseId;
        Assert.Equal(initialCourseId, courseEvent.CourseId);
    }

    [Fact]
    public void EventDate_Property_Should_Be_Read_Only()
    {
        // Arrange
        var eventDate = DateTime.UtcNow;
        var courseEvent = new CourseEvent(Guid.NewGuid(), Guid.NewGuid(), eventDate, 1000m, 30, 1, VenueType.InPerson);

        // Assert
        var initialEventDate = courseEvent.EventDate;
        Assert.Equal(initialEventDate, courseEvent.EventDate);
    }

    [Fact]
    public void Price_Property_Should_Be_Read_Only()
    {
        // Arrange
        var courseEvent = new CourseEvent(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, 1000m, 30, 1, VenueType.InPerson);

        // Assert
        var initialPrice = courseEvent.Price;
        Assert.Equal(initialPrice, courseEvent.Price);
    }

    [Fact]
    public void Seats_Property_Should_Be_Read_Only()
    {
        // Arrange
        var courseEvent = new CourseEvent(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, 1000m, 30, 1, VenueType.InPerson);

        // Assert
        var initialSeats = courseEvent.Seats;
        Assert.Equal(initialSeats, courseEvent.Seats);
    }

    [Fact]
    public void CourseEventTypeId_Property_Should_Be_Read_Only()
    {
        // Arrange
        var courseEvent = new CourseEvent(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, 1000m, 30, 1, VenueType.InPerson);

        // Assert
        var initialCourseEventTypeId = courseEvent.CourseEventTypeId;
        Assert.Equal(initialCourseEventTypeId, courseEvent.CourseEventTypeId);
    }

    [Fact]
    public void Constructor_Should_Handle_Future_Dates()
    {
        // Arrange
        var eventDate = DateTime.UtcNow.AddDays(30);
        var courseEvent = new CourseEvent(Guid.NewGuid(), Guid.NewGuid(), eventDate, 1000m, 30, 1, VenueType.InPerson);

        // Assert
        Assert.Equal(eventDate, courseEvent.EventDate);
    }

    [Fact]
    public void Constructor_Should_Handle_Past_Dates()
    {
        // Arrange
        var eventDate = DateTime.UtcNow.AddDays(-30);
        var courseEvent = new CourseEvent(Guid.NewGuid(), Guid.NewGuid(), eventDate, 1000m, 30, 1, VenueType.InPerson);

        // Assert
        Assert.Equal(eventDate, courseEvent.EventDate);
    }

    [Fact]
    public void Multiple_Events_Can_Have_Same_CourseId()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var event1 = new CourseEvent(Guid.NewGuid(), courseId, DateTime.UtcNow, 1000m, 30, 1, VenueType.InPerson);
        var event2 = new CourseEvent(Guid.NewGuid(), courseId, DateTime.UtcNow.AddDays(1), 1500m, 25, 1, VenueType.InPerson);

        // Assert
        Assert.Equal(courseId, event1.CourseId);
        Assert.Equal(courseId, event2.CourseId);
        Assert.NotEqual(event1.Id, event2.Id);
    }

    [Fact]
    public void Constructor_Should_Handle_Large_Seat_Capacity()
    {
        // Arrange
        var seats = 1000;
        var courseEvent = new CourseEvent(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, 1000m, seats, 1, VenueType.InPerson);

        // Assert
        Assert.Equal(seats, courseEvent.Seats);
    }

    [Fact]
    public void Constructor_Should_Handle_High_Prices()
    {
        // Arrange
        var price = 999999.99m;
        var courseEvent = new CourseEvent(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, price, 30, 1, VenueType.InPerson);

        // Assert
        Assert.Equal(price, courseEvent.Price);
    }
}
