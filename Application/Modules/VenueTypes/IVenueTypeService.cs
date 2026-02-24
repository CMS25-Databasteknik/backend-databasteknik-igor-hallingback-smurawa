using Backend.Application.Modules.VenueTypes.Inputs;
using Backend.Application.Modules.VenueTypes.Outputs;

namespace Backend.Application.Modules.VenueTypes;

public interface IVenueTypeService
{
    Task<VenueTypeListResult> GetAllVenueTypesAsync(CancellationToken cancellationToken = default);
    Task<VenueTypeResult> GetVenueTypeByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<VenueTypeResult> GetVenueTypeByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<VenueTypeResult> CreateVenueTypeAsync(CreateVenueTypeInput input, CancellationToken cancellationToken = default);
    Task<VenueTypeResult> UpdateVenueTypeAsync(UpdateVenueTypeInput input, CancellationToken cancellationToken = default);
    Task<VenueTypeDeleteResult> DeleteVenueTypeAsync(int id, CancellationToken cancellationToken = default);
}
