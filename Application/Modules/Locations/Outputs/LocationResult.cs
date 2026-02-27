using Backend.Application.Common;
using Backend.Domain.Modules.Locations.Models;

namespace Backend.Application.Modules.Locations.Outputs;

public sealed record LocationResult : ResultBase<Location>
{
}

public sealed record LocationListResult : ResultBase<IReadOnlyList<Location>>
{
}

public sealed record LocationDeleteResult : ResultBase<bool>
{
}
