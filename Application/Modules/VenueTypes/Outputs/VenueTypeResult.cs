using Backend.Application.Common;
using Backend.Domain.Modules.VenueTypes.Models;

namespace Backend.Application.Modules.VenueTypes.Outputs;

public sealed record VenueTypeResult : ResultBase<VenueType>
{
}

public sealed record VenueTypeListResult : ResultBase<IReadOnlyList<VenueType>>
{
}

public sealed record VenueTypeDeleteResult : ResultBase<bool>
{
}
