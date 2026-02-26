using Backend.Application.Common;
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
                    Error = ResultError.Validation,
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

            var result = await _locationRepository.AddAsync(newLocation, cancellationToken);

            return new LocationResult
            {
                Success = true,
                                Result = result,
                Message = "Location created successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new LocationResult
            {
                Success = false,
                Error = ResultError.Validation,
                Result = null,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new LocationResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Result = null,
                Message = $"An error occurred while creating the location: {ex.Message}"
            };
        }
    }

    public async Task<LocationListResult> GetAllLocationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var locations = await _locationRepository.GetAllAsync(cancellationToken);

            if (!locations.Any())
            {
                return new LocationListResult
                {
                    Success = true,
                    Result = locations,
                                        Message = "No locations found."
                };
            }

            return new LocationListResult
            {
                Success = true,
                                Result = locations,
                Message = $"Retrieved {locations.Count} location(s) successfully."
            };
        }
        catch (Exception ex)
        {
            return new LocationListResult
            {
                Success = false,
                Error = ResultError.Unexpected,
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
                    Error = ResultError.Validation,
                    Message = "Location ID must be greater than zero."
                };
            }

            var existingLocation = await _locationRepository.GetByIdAsync(locationId, cancellationToken);

            if (existingLocation == null)
            {
                return new LocationResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Location with ID '{locationId}' not found."
                };
            }

            return new LocationResult
            {
                Success = true,
                                Result = existingLocation,
                Message = "Location retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new LocationResult
            {
                Success = false,
                Error = ResultError.Unexpected,
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
                    Error = ResultError.Validation,
                    Message = "Location cannot be null."
                };
            }

            if (location.Id <= 0)
            {
                return new LocationResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Message = "Location ID must be greater than zero."
                };
            }

            var existingLocation = await _locationRepository.GetByIdAsync(location.Id, cancellationToken);
            if (existingLocation == null)
            {
                return new LocationResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Location with ID '{location.Id}' not found."
                };
            }

            existingLocation.Update(
                location.StreetName,
                location.PostalCode,
                location.City
            );

            var updatedLocation = await _locationRepository.UpdateAsync(existingLocation.Id, existingLocation, cancellationToken);

            if (updatedLocation == null)
            {
                return new LocationResult
                {
                    Success = false,
                    Error = ResultError.Unexpected,
                    Message = "Failed to update location."
                };
            }

            return new LocationResult
            {
                Success = true,
                                Result = updatedLocation,
                Message = "Location updated successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new LocationResult
            {
                Success = false,
                Error = ResultError.Validation,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new LocationResult
            {
                Success = false,
                Error = ResultError.Unexpected,
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
                    Error = ResultError.Validation,
                    Message = "Location ID must be greater than zero.",
                    Result = false
                };
            }

            var existingLocation = await _locationRepository.GetByIdAsync(locationId, cancellationToken);
            if (existingLocation == null)
            {
                return new LocationDeleteResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
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
                    Error = ResultError.Conflict,
                    Message = $"Cannot delete location with ID '{locationId}' because it has in-place locations. Please delete the in-place locations first.",
                    Result = false
                };
            }

            var result = await _locationRepository.RemoveAsync(locationId, cancellationToken);

            if (!result)
            {
                return new LocationDeleteResult
                {
                    Success = false,
                    Error = ResultError.Unexpected,
                    Message = "Failed to delete location.",
                    Result = false
                };
            }

            return new LocationDeleteResult
            {
                Success = true,
                                Result = result,
                Message = "Location deleted successfully."
            };
        }
        catch (Exception ex)
        {
            return new LocationDeleteResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Message = $"An error occurred while deleting the location: {ex.Message}",
                Result = false
            };
        }
    }
}

