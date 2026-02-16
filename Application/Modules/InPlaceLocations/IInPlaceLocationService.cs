using Backend.Application.Modules.InPlaceLocations.Inputs;
using Backend.Application.Modules.InPlaceLocations.Outputs;

namespace Backend.Application.Modules.InPlaceLocations;

public interface IInPlaceLocationService
{
    Task<InPlaceLocationResult> CreateInPlaceLocationAsync(CreateInPlaceLocationInput inPlaceLocation, CancellationToken cancellationToken = default);
    Task<InPlaceLocationListResult> GetAllInPlaceLocationsAsync(CancellationToken cancellationToken = default);
    Task<InPlaceLocationResult> GetInPlaceLocationByIdAsync(int inPlaceLocationId, CancellationToken cancellationToken = default);
    Task<InPlaceLocationListResult> GetInPlaceLocationsByLocationIdAsync(int locationId, CancellationToken cancellationToken = default);
    Task<InPlaceLocationResult> UpdateInPlaceLocationAsync(UpdateInPlaceLocationInput inPlaceLocation, CancellationToken cancellationToken = default);
    Task<InPlaceLocationDeleteResult> DeleteInPlaceLocationAsync(int inPlaceLocationId, CancellationToken cancellationToken = default);
}
