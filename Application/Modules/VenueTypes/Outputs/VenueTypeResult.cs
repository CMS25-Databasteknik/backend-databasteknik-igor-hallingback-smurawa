using Backend.Application.Common;
using Backend.Domain.Modules.VenueTypes.Models;

namespace Backend.Application.Modules.VenueTypes.Outputs;

public sealed class VenueTypeResult : ResultCommon<VenueType>
{
}

public sealed class VenueTypeListResult : ResultCommon<IReadOnlyList<VenueType>>
{
}

public sealed class VenueTypeDeleteResult : ResultCommon<bool>
{
}
