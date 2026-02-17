using Backend.Application.Modules.Locations.Inputs;
using Backend.Application.Modules.Locations.Outputs;
using Backend.Domain.Modules.Locations.Contracts;
using Backend.Domain.Modules.Locations.Models;

namespace Backend.Application.Modules.Locations;

public class LocationService(ILocationRepository locationRepository) : ILocationService
{
    private readonly ILocationRepository _locationRepository = locationRepository ?? throw new ArgumentNullException(nameof(locationRepository));

    public async Task<LocationResult> CreateLocationAsync(CreateLocationInput location, CancellationToken cancellationToken = default)
    {
        try
        {
            if (location == null)
            {
                return new LocationResult
                {
                    Success = false,
                    StatusCode = 400,
                    Result = null,
                    Message = "Location cannot be null."
                };
            }

            var newLocation = new Location(
                0,
                location.StreetName,
                location.PostalCode,
                location.City
            );

            var result = await _locationRepository.CreateLocationAsync(newLocation, cancellationToken);

            return new LocationResult
            {
                Success = true,
                StatusCode = 201,
                Result = result,
                Message = "Location created successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new LocationResult
            {
                Success = false,
                StatusCode = 400,
                Result = null,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new LocationResult
            {
                Success = false,
                StatusCode = 500,
                Result = null,
                Message = $"An error occurred while creating the location: {ex.Message}"
            };
        }
    }

    public async Task<LocationListResult> GetAllLocationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var locations = await _locationRepository.GetAllLocationsAsync(cancellationToken);

            if (!locations.Any())
            {
                return new LocationListResult
                {
                    Success = true,
                    Result = locations,
                    StatusCode = 200,
                    Message = "No locations found."
                };
            }

            return new LocationListResult
            {
                Success = true,
                StatusCode = 200,
                Result = locations,
                Message = $"Retrieved {locations.Count} location(s) successfully."
            };
        }
        catch (Exception ex)
        {
            return new LocationListResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving locations: {ex.Message}"
            };
        }
    }

    public async Task<LocationResult> GetLocationByIdAsync(int locationId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (locationId <= 0)
            {
                return new LocationResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Location ID must be greater than zero."
                };
            }

            var result = await _locationRepository.GetLocationByIdAsync(locationId, cancellationToken);

            if (result == null)
            {
                return new LocationResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Location with ID '{locationId}' not found."
                };
            }

            return new LocationResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "Location retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new LocationResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving the location: {ex.Message}"
            };
        }
    }

    public async Task<LocationResult> UpdateLocationAsync(UpdateLocationInput location, CancellationToken cancellationToken = default)
    {
        try
        {
            if (location == null)
            {
                return new LocationResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Location cannot be null."
                };
            }

            if (location.Id <= 0)
            {
                return new LocationResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Location ID must be greater than zero."
                };
            }

            var existingLocation = await _locationRepository.GetLocationByIdAsync(location.Id, cancellationToken);
            if (existingLocation == null)
            {
                return new LocationResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Location with ID '{location.Id}' not found."
                };
            }

            var updatedLocation = new Location(
                location.Id,
                location.StreetName,
                location.PostalCode,
                location.City
            );

            var result = await _locationRepository.UpdateLocationAsync(updatedLocation, cancellationToken);

            if (result == null)
            {
                return new LocationResult
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to update location."
                };
            }

            return new LocationResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "Location updated successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new LocationResult
            {
                Success = false,
                StatusCode = 400,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new LocationResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while updating the location: {ex.Message}"
            };
        }
    }

    public async Task<LocationDeleteResult> DeleteLocationAsync(int locationId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (locationId <= 0)
            {
                return new LocationDeleteResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Location ID must be greater than zero.",
                    Result = false
                };
            }

            var existingLocation = await _locationRepository.GetLocationByIdAsync(locationId, cancellationToken);
            if (existingLocation == null)
            {
                return new LocationDeleteResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Location with ID '{locationId}' not found.",
                    Result = false
                };
            }

            var hasInPlaceLocations = await _locationRepository.HasInPlaceLocationsAsync(locationId, cancellationToken);
            if (hasInPlaceLocations)
            {
                return new LocationDeleteResult
                {
                    Success = false,
                    StatusCode = 409,
                    Message = $"Cannot delete location with ID '{locationId}' because it has in-place locations. Please delete the in-place locations first.",
                    Result = false
                };
            }

            var result = await _locationRepository.DeleteLocationAsync(locationId, cancellationToken);

            if (!result)
            {
                return new LocationDeleteResult
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to delete location.",
                    Result = false
                };
            }

            return new LocationDeleteResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "Location deleted successfully."
            };
        }
        catch (Exception ex)
        {
            return new LocationDeleteResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while deleting the location: {ex.Message}",
                Result = false
            };
        }
    }
}
