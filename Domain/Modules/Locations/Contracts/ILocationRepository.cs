using Backend.Domain.Modules.Locations.Models;

namespace Backend.Domain.Modules.Locations.Contracts;

public interface ILocationRepository
{
    Task<Location> CreateLocationAsync(Location location, CancellationToken cancellationToken);
    Task<Location?> GetLocationByIdAsync(int locationId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Location>> GetAllLocationsAsync(CancellationToken cancellationToken);
    Task<Location?> UpdateLocationAsync(Location location, CancellationToken cancellationToken);
    Task<bool> DeleteLocationAsync(int locationId, CancellationToken cancellationToken);
    Task<bool> HasInPlaceLocationsAsync(int locationId, CancellationToken cancellationToken);
}
