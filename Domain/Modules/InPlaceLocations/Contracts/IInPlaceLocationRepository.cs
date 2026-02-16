using Backend.Domain.Modules.InPlaceLocations.Models;

namespace Backend.Domain.Modules.InPlaceLocations.Contracts;

public interface IInPlaceLocationRepository
{
    Task<InPlaceLocation> CreateInPlaceLocationAsync(InPlaceLocation inPlaceLocation, CancellationToken cancellationToken);
    Task<InPlaceLocation?> GetInPlaceLocationByIdAsync(int inPlaceLocationId, CancellationToken cancellationToken);
    Task<IReadOnlyList<InPlaceLocation>> GetAllInPlaceLocationsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<InPlaceLocation>> GetInPlaceLocationsByLocationIdAsync(int locationId, CancellationToken cancellationToken);
    Task<InPlaceLocation?> UpdateInPlaceLocationAsync(InPlaceLocation inPlaceLocation, CancellationToken cancellationToken);
    Task<bool> DeleteInPlaceLocationAsync(int inPlaceLocationId, CancellationToken cancellationToken);
    Task<bool> HasCourseEventsAsync(int inPlaceLocationId, CancellationToken cancellationToken);
}
