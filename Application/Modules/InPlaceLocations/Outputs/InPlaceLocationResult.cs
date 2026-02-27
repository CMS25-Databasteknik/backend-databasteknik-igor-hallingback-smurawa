using Backend.Application.Common;
using Backend.Domain.Modules.InPlaceLocations.Models;

namespace Backend.Application.Modules.InPlaceLocations.Outputs;

public sealed record InPlaceLocationResult : ResultBase<InPlaceLocation>
{
}

public sealed record InPlaceLocationListResult : ResultBase<IReadOnlyList<InPlaceLocation>>
{
}

public sealed record InPlaceLocationDeleteResult : ResultBase<bool>
{
}
