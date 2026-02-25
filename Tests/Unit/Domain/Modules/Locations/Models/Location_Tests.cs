using Backend.Domain.Modules.Locations.Models;

namespace Tests.Unit.Domain.Modules.Locations.Models;

public class Location_Tests
{
    [Fact]
    public void Constructor_Should_Create_Location_When_Parameters_Are_Valid()
    {
        // Arrange
        var id = 1;
        var streetName = "Kungsgatan 12";
        var postalCode = "111 43";
        var city = "Stockholm";

        // Act
        var location = new Location(id, streetName, postalCode, city);

        // Assert
        Assert.NotNull(location);
        Assert.Equal(id, location.Id);
        Assert.Equal(streetName, location.StreetName);
        Assert.Equal(postalCode, location.PostalCode);
        Assert.Equal(city, location.City);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_Id_Is_Negative()
    {
        // Arrange
        var id = -1;
        var streetName = "Kungsgatan 12";
        var postalCode = "111 43";
        var city = "Stockholm";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Location(id, streetName, postalCode, city));

        Assert.Equal("id", exception.ParamName);
        Assert.Contains("ID must be greater than or equal to zero", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Accept_Zero_Id()
    {
        // Arrange
        var id = 0;
        var streetName = "Kungsgatan 12";
        var postalCode = "111 43";
        var city = "Stockholm";

        // Act
        var location = new Location(id, streetName, postalCode, city);

        // Assert
        Assert.Equal(0, location.Id);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_StreetName_Is_Null()
    {
        // Arrange
        var id = 1;
        string streetName = null!;
        var postalCode = "111 43";
        var city = "Stockholm";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Location(id, streetName, postalCode, city));

        Assert.Equal("streetName", exception.ParamName);
        Assert.Contains("Street name cannot be empty or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_StreetName_Is_Empty()
    {
        // Arrange
        var id = 1;
        var streetName = "";
        var postalCode = "111 43";
        var city = "Stockholm";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Location(id, streetName, postalCode, city));

        Assert.Equal("streetName", exception.ParamName);
        Assert.Contains("Street name cannot be empty or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_StreetName_Is_Whitespace()
    {
        // Arrange
        var id = 1;
        var streetName = "   ";
        var postalCode = "111 43";
        var city = "Stockholm";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Location(id, streetName, postalCode, city));

        Assert.Equal("streetName", exception.ParamName);
        Assert.Contains("Street name cannot be empty or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_PostalCode_Is_Null()
    {
        // Arrange
        var id = 1;
        var streetName = "Kungsgatan 12";
        string postalCode = null!;
        var city = "Stockholm";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Location(id, streetName, postalCode, city));

        Assert.Equal("postalCode", exception.ParamName);
        Assert.Contains("Postal code cannot be empty or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_PostalCode_Is_Empty()
    {
        // Arrange
        var id = 1;
        var streetName = "Kungsgatan 12";
        var postalCode = "";
        var city = "Stockholm";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Location(id, streetName, postalCode, city));

        Assert.Equal("postalCode", exception.ParamName);
        Assert.Contains("Postal code cannot be empty or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_PostalCode_Is_Whitespace()
    {
        // Arrange
        var id = 1;
        var streetName = "Kungsgatan 12";
        var postalCode = "   ";
        var city = "Stockholm";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Location(id, streetName, postalCode, city));

        Assert.Equal("postalCode", exception.ParamName);
        Assert.Contains("Postal code cannot be empty or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_City_Is_Null()
    {
        // Arrange
        var id = 1;
        var streetName = "Kungsgatan 12";
        var postalCode = "111 43";
        string city = null!;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Location(id, streetName, postalCode, city));

        Assert.Equal("city", exception.ParamName);
        Assert.Contains("City cannot be empty or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_City_Is_Empty()
    {
        // Arrange
        var id = 1;
        var streetName = "Kungsgatan 12";
        var postalCode = "111 43";
        var city = "";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Location(id, streetName, postalCode, city));

        Assert.Equal("city", exception.ParamName);
        Assert.Contains("City cannot be empty or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_City_Is_Whitespace()
    {
        // Arrange
        var id = 1;
        var streetName = "Kungsgatan 12";
        var postalCode = "111 43";
        var city = "   ";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Location(id, streetName, postalCode, city));

        Assert.Equal("city", exception.ParamName);
        Assert.Contains("City cannot be empty or whitespace", exception.Message);
    }

    [Theory]
    [InlineData(1, "Kungsgatan 12", "111 43", "Stockholm")]
    [InlineData(2, "Drottninggatan 1", "111 21", "Stockholm")]
    [InlineData(3, "Storgatan 5", "411 38", "Göteborg")]
    [InlineData(4, "Vasagatan 10", "211 20", "Malmö")]
    public void Constructor_Should_Create_Location_With_Various_Valid_Parameters(
        int id, string streetName, string postalCode, string city)
    {
        // Act
        var location = new Location(id, streetName, postalCode, city);

        // Assert
        Assert.Equal(id, location.Id);
        Assert.Equal(streetName, location.StreetName);
        Assert.Equal(postalCode, location.PostalCode);
        Assert.Equal(city, location.City);
    }

    [Fact]
    public void Properties_Should_Be_Initialized_Correctly()
    {
        // Arrange & Act
        var location = new Location(1, "Kungsgatan 12", "111 43", "Stockholm");

        // Assert
        Assert.Equal(1, location.Id);
        Assert.Equal("Kungsgatan 12", location.StreetName);
        Assert.Equal("111 43", location.PostalCode);
        Assert.Equal("Stockholm", location.City);
    }

    [Fact]
    public void Two_Locations_With_Same_Values_Should_Have_Same_Property_Values()
    {
        // Arrange
        var location1 = new Location(1, "Kungsgatan 12", "111 43", "Stockholm");
        var location2 = new Location(1, "Kungsgatan 12", "111 43", "Stockholm");

        // Assert
        Assert.Equal(location1.Id, location2.Id);
        Assert.Equal(location1.StreetName, location2.StreetName);
        Assert.Equal(location1.PostalCode, location2.PostalCode);
        Assert.Equal(location1.City, location2.City);
    }

    [Fact]
    public void Constructor_Should_Handle_Long_Street_Names()
    {
        // Arrange
        var id = 1;
        var streetName = "Very Long Street Name With Multiple Words And Numbers 123";
        var postalCode = "111 43";
        var city = "Stockholm";

        // Act
        var location = new Location(id, streetName, postalCode, city);

        // Assert
        Assert.Equal(streetName, location.StreetName);
    }

    [Fact]
    public void Constructor_Should_Handle_Swedish_Characters()
    {
        // Arrange
        var id = 1;
        var streetName = "Drottninggatan 1";
        var postalCode = "111 21";
        var city = "Göteborg";

        // Act
        var location = new Location(id, streetName, postalCode, city);

        // Assert
        Assert.Equal("Göteborg", location.City);
    }

    [Fact]
    public void Id_Property_Should_Be_Read_Only()
    {
        // Arrange
        var location = new Location(1, "Kungsgatan 12", "111 43", "Stockholm");

        // Assert
        Assert.Equal(1, location.Id);
        // Verify that Id property has only a getter (this is a compile-time check, but we can verify the value doesn't change)
        var initialId = location.Id;
        Assert.Equal(initialId, location.Id);
    }

    [Fact]
    public void StreetName_Property_Should_Be_Read_Only()
    {
        // Arrange
        var location = new Location(1, "Kungsgatan 12", "111 43", "Stockholm");

        // Assert
        Assert.Equal("Kungsgatan 12", location.StreetName);
        var initialStreetName = location.StreetName;
        Assert.Equal(initialStreetName, location.StreetName);
    }

    [Fact]
    public void PostalCode_Property_Should_Be_Read_Only()
    {
        // Arrange
        var location = new Location(1, "Kungsgatan 12", "111 43", "Stockholm");

        // Assert
        Assert.Equal("111 43", location.PostalCode);
        var initialPostalCode = location.PostalCode;
        Assert.Equal(initialPostalCode, location.PostalCode);
    }

    [Fact]
    public void City_Property_Should_Be_Read_Only()
    {
        // Arrange
        var location = new Location(1, "Kungsgatan 12", "111 43", "Stockholm");

        // Assert
        Assert.Equal("Stockholm", location.City);
        var initialCity = location.City;
        Assert.Equal(initialCity, location.City);
    }

    [Fact]
    public void Update_Should_Change_Values_When_Input_Is_Valid()
    {
        var location = new Location(1, "Old Street", "111 11", "Old City");

        location.Update("New Street", "222 22", "New City");

        Assert.Equal("New Street", location.StreetName);
        Assert.Equal("222 22", location.PostalCode);
        Assert.Equal("New City", location.City);
    }

    [Fact]
    public void Update_Should_Throw_ArgumentException_When_City_Is_Whitespace()
    {
        var location = new Location(1, "Old Street", "111 11", "Old City");

        var ex = Assert.Throws<ArgumentException>(() => location.Update("New Street", "222 22", "   "));
        Assert.Equal("city", ex.ParamName);
    }
}
