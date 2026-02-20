using Backend.Application.Common;
using Backend.Domain.Modules.Locations.Models;

namespace Backend.Application.Modules.Locations.Outputs;

public sealed class LocationResult : ResultCommon<Location>
{
}

public sealed class LocationListResult : ResultCommon<IReadOnlyList<Location>>
{
}

public sealed class LocationDeleteResult : ResultCommon<bool>
{
}
