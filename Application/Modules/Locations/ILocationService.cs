using Backend.Application.Modules.Locations.Inputs;
using Backend.Application.Modules.Locations.Outputs;

namespace Backend.Application.Modules.Locations;

public interface ILocationService
{
    Task<LocationResult> CreateLocationAsync(CreateLocationInput location, CancellationToken cancellationToken = default);
    Task<LocationListResult> GetAllLocationsAsync(CancellationToken cancellationToken = default);
    Task<LocationResult> GetLocationByIdAsync(int locationId, CancellationToken cancellationToken = default);
    Task<LocationResult> UpdateLocationAsync(UpdateLocationInput location, CancellationToken cancellationToken = default);
    Task<LocationDeleteResult> DeleteLocationAsync(int locationId, CancellationToken cancellationToken = default);
}
