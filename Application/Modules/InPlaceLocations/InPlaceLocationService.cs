using Backend.Application.Modules.InPlaceLocations.Inputs;
using Backend.Application.Modules.InPlaceLocations.Outputs;
using Backend.Domain.Modules.InPlaceLocations.Contracts;
using Backend.Domain.Modules.InPlaceLocations.Models;

namespace Backend.Application.Modules.InPlaceLocations;

public class InPlaceLocationService(IInPlaceLocationRepository inPlaceLocationRepository) : IInPlaceLocationService
{
    private readonly IInPlaceLocationRepository _inPlaceLocationRepository = inPlaceLocationRepository ?? throw new ArgumentNullException(nameof(inPlaceLocationRepository));

    public async Task<InPlaceLocationResult> CreateInPlaceLocationAsync(CreateInPlaceLocationInput inPlaceLocation, CancellationToken cancellationToken = default)
    {
        try
        {
            if (inPlaceLocation == null)
            {
                return new InPlaceLocationResult
                {
                    Success = false,
                    StatusCode = 400,
                    Result = null,
                    Message = "In-place location cannot be null."
                };
            }

            var newInPlaceLocation = new InPlaceLocation(
                0,
                inPlaceLocation.LocationId,
                inPlaceLocation.RoomNumber,
                inPlaceLocation.Seats
            );

            var result = await _inPlaceLocationRepository.AddAsync(newInPlaceLocation, cancellationToken);

            return new InPlaceLocationResult
            {
                Success = true,
                StatusCode = 201,
                Result = result,
                Message = "In-place location created successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new InPlaceLocationResult
            {
                Success = false,
                StatusCode = 400,
                Result = null,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new InPlaceLocationResult
            {
                Success = false,
                StatusCode = 500,
                Result = null,
                Message = $"An error occurred while creating the in-place location: {ex.Message}"
            };
        }
    }

    public async Task<InPlaceLocationListResult> GetAllInPlaceLocationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var inPlaceLocations = await _inPlaceLocationRepository.GetAllAsync(cancellationToken);

            if (!inPlaceLocations.Any())
            {
                return new InPlaceLocationListResult
                {
                    Success = true,
                    Result = inPlaceLocations,
                    StatusCode = 200,
                    Message = "No in-place locations found."
                };
            }

            return new InPlaceLocationListResult
            {
                Success = true,
                StatusCode = 200,
                Result = inPlaceLocations,
                Message = $"Retrieved {inPlaceLocations.Count} in-place location(s) successfully."
            };
        }
        catch (Exception ex)
        {
            return new InPlaceLocationListResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving in-place locations: {ex.Message}"
            };
        }
    }

    public async Task<InPlaceLocationResult> GetInPlaceLocationByIdAsync(int inPlaceLocationId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (inPlaceLocationId <= 0)
            {
                return new InPlaceLocationResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "In-place location ID must be greater than zero."
                };
            }

            var result = await _inPlaceLocationRepository.GetByIdAsync(inPlaceLocationId, cancellationToken);

            if (result == null)
            {
                return new InPlaceLocationResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"In-place location with ID '{inPlaceLocationId}' not found."
                };
            }

            return new InPlaceLocationResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "In-place location retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new InPlaceLocationResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving the in-place location: {ex.Message}"
            };
        }
    }

    public async Task<InPlaceLocationListResult> GetInPlaceLocationsByLocationIdAsync(int locationId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (locationId <= 0)
            {
                return new InPlaceLocationListResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Location ID must be greater than zero."
                };
            }

            var inPlaceLocations = await _inPlaceLocationRepository.GetInPlaceLocationsByLocationIdAsync(locationId, cancellationToken);

            if (!inPlaceLocations.Any())
            {
                return new InPlaceLocationListResult
                {
                    Success = true,
                    Result = inPlaceLocations,
                    StatusCode = 200,
                    Message = "No in-place locations found for this location."
                };
            }

            return new InPlaceLocationListResult
            {
                Success = true,
                StatusCode = 200,
                Result = inPlaceLocations,
                Message = $"Retrieved {inPlaceLocations.Count} in-place location(s) for the location successfully."
            };
        }
        catch (Exception ex)
        {
            return new InPlaceLocationListResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving in-place locations: {ex.Message}"
            };
        }
    }

    public async Task<InPlaceLocationResult> UpdateInPlaceLocationAsync(UpdateInPlaceLocationInput inPlaceLocation, CancellationToken cancellationToken = default)
    {
        try
        {
            if (inPlaceLocation == null)
            {
                return new InPlaceLocationResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "In-place location cannot be null."
                };
            }

            if (inPlaceLocation.Id <= 0)
            {
                return new InPlaceLocationResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "In-place location ID must be greater than zero."
                };
            }

            var existingInPlaceLocation = await _inPlaceLocationRepository.GetByIdAsync(inPlaceLocation.Id, cancellationToken);
            if (existingInPlaceLocation == null)
            {
                return new InPlaceLocationResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"In-place location with ID '{inPlaceLocation.Id}' not found."
                };
            }

            existingInPlaceLocation.Update(
                inPlaceLocation.LocationId,
                inPlaceLocation.RoomNumber,
                inPlaceLocation.Seats
            );

            var updatedInPlaceLocation = await _inPlaceLocationRepository.UpdateAsync(existingInPlaceLocation.Id, existingInPlaceLocation, cancellationToken);

            if (updatedInPlaceLocation == null)
            {
                return new InPlaceLocationResult
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to update in-place location."
                };
            }

            return new InPlaceLocationResult
            {
                Success = true,
                StatusCode = 200,
                Result = updatedInPlaceLocation,
                Message = "In-place location updated successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new InPlaceLocationResult
            {
                Success = false,
                StatusCode = 400,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new InPlaceLocationResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while updating the in-place location: {ex.Message}"
            };
        }
    }

    public async Task<InPlaceLocationDeleteResult> DeleteInPlaceLocationAsync(int inPlaceLocationId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (inPlaceLocationId <= 0)
            {
                return new InPlaceLocationDeleteResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "In-place location ID must be greater than zero.",
                    Result = false
                };
            }

            var existingInPlaceLocation = await _inPlaceLocationRepository.GetByIdAsync(inPlaceLocationId, cancellationToken);
            if (existingInPlaceLocation == null)
            {
                return new InPlaceLocationDeleteResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"In-place location with ID '{inPlaceLocationId}' not found.",
                    Result = false
                };
            }

            var hasCourseEvents = await _inPlaceLocationRepository.HasCourseEventsAsync(inPlaceLocationId, cancellationToken);
            if (hasCourseEvents)
            {
                return new InPlaceLocationDeleteResult
                {
                    Success = false,
                    StatusCode = 409,
                    Message = $"Cannot delete in-place location with ID '{inPlaceLocationId}' because it is assigned to course events. Please remove the assignments first.",
                    Result = false
                };
            }

            var result = await _inPlaceLocationRepository.RemoveAsync(inPlaceLocationId, cancellationToken);

            if (!result)
            {
                return new InPlaceLocationDeleteResult
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to delete in-place location.",
                    Result = false
                };
            }

            return new InPlaceLocationDeleteResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "In-place location deleted successfully."
            };
        }
        catch (Exception ex)
        {
            return new InPlaceLocationDeleteResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while deleting the in-place location: {ex.Message}",
                Result = false
            };
        }
    }
}

