using Backend.Application.Common;
using Backend.Domain.Modules.InPlaceLocations.Models;

namespace Backend.Application.Modules.InPlaceLocations.Outputs;

public sealed class InPlaceLocationResult : ResultCommon<InPlaceLocation>
{
}

public sealed class InPlaceLocationListResult : ResultCommon<IReadOnlyList<InPlaceLocation>>
{
}

public sealed class InPlaceLocationDeleteResult : ResultCommon<bool>
{
}
